using AtomicHabits.Models;
using AtomicHabits.Models.DTO;
using AtomicHabits.Repositories;
using AtomicHabits.Service;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace AtomicHabits.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterDto dto);
        Task<ApiResponse> LoginAsync(LoginDto dto);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken);
        Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct);
        //Task<ApiResponse> StoreRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default);
        Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken);

        Task<UserInfoDto?> GetCurrentUserFromJwt(string? jwtToken);
        Task<string?> GetUserIdFromJwt(string? jwtToken);
        Task<string?> GetUserIdAsync(HttpContext httpContext);
        Task<UserInfoDto?> GetCurrentUserAsync(HttpContext httpContext);
        Task<ApiResponse> RevokeRefreshTokenAsync(int userId, CancellationToken ct);

    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IUserRepositories _userRepo;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthService> _logger;
        private ApiResponse _response;
        private readonly IPasswordHasher<User> _hasher;

        public AuthService(AppDbContext db, ITokenService tokenService, IUserRepositories userRepo, IEmailSender emailSender, ILogger<AuthService> logger, IPasswordHasher<User> hasher)
        {
            _db = db;
            _tokenService = tokenService;
            _userRepo = userRepo;
            _emailSender = emailSender;
            _logger = logger;
            _response = new ApiResponse();
            _hasher = hasher;
        }

        #region Main Feature 

        public async Task<ApiResponse> RegisterAsync(RegisterDto dto)
        {
            using var trx = await _db.Database.BeginTransactionAsync();
            try
            {
                if (await _db.Users.AnyAsync(x => x.Username == dto.Username || x.Email == dto.Email))
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username or Email already exists");
                    return _response;
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                // assign role if exists
                var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role);
                if (role != null)
                {
                    await _db.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
                    await _db.SaveChangesAsync();
                }

                return await IssueTokensAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.RegisterAsync] Error for {Username}", dto?.Username);
                await trx.RollbackAsync();
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }
        }

        public async Task<ApiResponse> LoginAsync(LoginDto dto)
        {
            var response = new ApiResponse();
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Username and password are required.");
                    return response;
                }

                var identifier = dto.Email.Trim();
                var user = await _userRepo.GetByEmailAsync(identifier);
                if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Invalid credentials.");
                    return response;
                }

                return await IssueTokensAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.LoginAsync] Error login attempt for {Username}", dto?.Email);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) return null;

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var success = await _userRepo.SetPasswordResetTokenAsync(user, token);
            return success ? token : null;
        }

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("User not found.");
                    return _response;
                }

                var token = await GeneratePasswordResetTokenAsync(dto.Email);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var resetPath = "http://localhost:3000/ResetPassword?" + $"token={token}&" + $"email={user.Email}";

                await _emailSender.SendEmailAsync(dto.Email, "Reset Password", MailBody(dto.Email, HtmlEncoder.Default.Encode(resetPath)));

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = new ForgotPasswordDTO();
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.ForgotPasswordAsync] Error for {Email}", dto.Email);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("User not found.");
                return _response;
            }

            var decodedToken = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
            if (user.PasswordResetToken != decodedToken || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Invalid or expired reset token.");
                return _response;
            }

            var setToken = await _userRepo.SetPasswordResetTokenAsync(user, decodedToken);
            if (!setToken)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add("Failed to set reset token state.");
                return _response;
            }

            var updateResult = await _userRepo.UpdatePasswordAsync(user, dto.Password);
            if (!updateResult)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add("Failed to update password");
                return _response;
            }

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        #endregion


        #region Token Section

        private async Task<ApiResponse> IssueTokensAsync(User user)
        {
            var roles = await _db.UserRoles
                .Where(x => x.UserId == user.Id)
                .Include(x => x.Role)
                .Select(x => x.Role.Name)
                .ToListAsync();

            var accessToken = await _tokenService.GenerateToken(user, roles);
            var refreshToken = await _tokenService.GenerateRefreshToken();

            var refreshEntity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = Hash(refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _db.RefreshTokens.Add(refreshEntity);
            await _db.SaveChangesAsync();

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = new
            {
                accessToken,
                refreshToken,
                expiresAt = DateTime.UtcNow.AddDays(1)
            };

            return _response;
        }

        public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken ct)
        {
            try
            {
                var hash = Hash(dto.RefreshToken);

                var token = await _db.RefreshTokens
                    .Include(t => t.User)
                    .ThenInclude(u => u.UserRoles)!
                    .ThenInclude(r => r.Role)
                    .FirstOrDefaultAsync(t => t.TokenHash == hash && !t.IsRevoked);

                if (token == null || token.ExpiresAt < DateTime.UtcNow)
                {
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Invalid refresh token");
                    return _response;
                }

                token.IsRevoked = true;
                var roles = token.User.UserRoles!.Select(r => r.Role.Name).ToList();
                var newAccess = await _tokenService.GenerateToken(token.User, roles);
                var newRefresh = _tokenService.GenerateRefreshToken();

                _db.RefreshTokens.Add(new RefreshToken
                {
                    UserId = token.UserId,
                    TokenHash = Hash(newRefresh.ToString()!),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(1)
                });

                await _db.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = new
                {
                    accessToken = newAccess,
                    refreshToken = newRefresh
                };
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.RefreshTokenAsync] Error refreshing token, message: " + ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error refresh token, message: " + ex.Message);
                return _response;
            }
        }

        public async Task<ApiResponse> RevokeRefreshTokenAsync(int userId, CancellationToken ct)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add("Username or Email already exists");
                    return _response;
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;

                await _db.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                return _response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.RevokeRefreshTokenAsync] Error revoke token, message: " + ex.Message);

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add("Error revoke token, message: " + ex.Message);
                return _response;
            }
        }

        //private static string HashToken(string token)
        //{
        //    using var sha = System.Security.Cryptography.SHA256.Create();
        //    var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        //    var hash = sha.ComputeHash(bytes);
        //    return Convert.ToBase64String(hash);
        //}

        //public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
        //{
        //    var response = new ApiResponse();
        //    try
        //    {
        //        #region validations

        //        if (string.IsNullOrEmpty(dto.RefreshToken))
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.IsSuccess = false;
        //            _response.ErrorMessages.Add("Refresh token is required");
        //            return response;
        //        }

        //        var userIdStr = _tokenService.GetUserIdFromToken(dto.RefreshToken);
        //        if (string.IsNullOrEmpty(userIdStr))
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.IsSuccess = false;
        //            _response.ErrorMessages.Add("Invalid refresh token");
        //            return response;
        //        }

        //        if (!int.TryParse(userIdStr, out var userId))
        //        {
        //            _response.StatusCode = HttpStatusCode.BadRequest;
        //            _response.IsSuccess = false;
        //            _response.ErrorMessages.Add("Invalid refresh token payload");
        //            return response;
        //        }

        //        var user = await _db.Users
        //            .Include(u => u.UserRoles)!
        //            .ThenInclude(ur => ur.Role)
        //            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        //        if (user == null)
        //        {
        //            _response.StatusCode = HttpStatusCode.NotFound;
        //            _response.IsSuccess = false;
        //            _response.ErrorMessages.Add("User not found");
        //            return response;
        //        }

        //        if (user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
        //        {
        //            _response.StatusCode = HttpStatusCode.Unauthorized;
        //            _response.IsSuccess = false;
        //            _response.ErrorMessages.Add("Invalid or expired refresh token");
        //            return response;
        //        }

        //        #endregion

        //        var roles = user.UserRoles!.Select(u => u.Role.Name).ToList();
        //        var newAccessToken = await _tokenService.GenerateToken(user, roles);
        //        var newRefreshToken = await _tokenService.GenerateRefreshToken(user);

        //        //user.RefreshToken = newRefreshToken;
        //        user.RefreshToken = HashToken(newRefreshToken); // store hashed new refresh token
        //        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        //        _db.Users.Update(user);
        //        await _db.SaveChangesAsync(cancellationToken);

        //        _response.StatusCode = HttpStatusCode.OK;
        //        _response.IsSuccess = true;
        //        _response.Result = new
        //        {
        //            accessToken = newAccessToken,
        //            refreshToken = newRefreshToken,
        //            expiresAt = DateTime.UtcNow.AddMinutes(15)
        //        };
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "[AuthService.RefreshTokenAsync] Error");
        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.ErrorMessages.Add(ex.Message);
        //        return response;
        //    }
        //}

        //public async Task<ApiResponse> StoreRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default)
        //{
        //    var response = new ApiResponse();
        //    try
        //    {
        //        var user = await _db.Users.FindAsync(new object[] { userId }, cancellationToken);
        //        if (user != null)
        //        {
        //            //user.RefreshToken = refreshToken;
        //            user.RefreshToken = HashToken(refreshToken); // store hashed token
        //            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(3);
        //            await _db.SaveChangesAsync(cancellationToken);

        //            _response.IsSuccess = true;
        //            _response.StatusCode = HttpStatusCode.OK;
        //            _response.Result = new
        //            {
        //                user.RefreshToken, user.RefreshTokenExpiry
        //            };

        //            return response;
        //        }

        //        _response.StatusCode = HttpStatusCode.NotFound;
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages.Add("User not found, please get any user that exists.");
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "[AuthService.StoreRefreshTokenAsync] Error storing refresh token for {UserId}", userId);

        //        _response.IsSuccess = false;
        //        _response.StatusCode = HttpStatusCode.InternalServerError;
        //        _response.ErrorMessages = new List<string>() { ex.Message.ToString() };
        //        return response;
        //    }
        //}

        #endregion

        #region JWT Token functions
        public async Task<UserInfoDto?> GetCurrentUserFromJwt(string? jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken)) return null;

                var claims = _tokenService.GetClaimsFromToken(jwtToken);
                if (claims == null || claims.Count == 0) return null;

                var userId = claims.TryGetValue(ClaimTypes.NameIdentifier, out var id) ? id : null;
                if (string.IsNullOrWhiteSpace(userId)) return null;
                var email = claims.TryGetValue(ClaimTypes.Email, out var e) ? e : string.Empty;
                var username = claims.TryGetValue("username", out var u) ? u : string.Empty;

                if (string.IsNullOrWhiteSpace(userId)) return null;

                return new UserInfoDto
                {
                    UserId = userId!,
                    Email = email ?? string.Empty,
                    Name = username ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.GetCurrentUserFromJwt] Error parsing token");
                return null;
            }
        }

        public async Task<string?> GetUserIdFromJwt(string? jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken)) return null;
                var userId = _tokenService.GetUserIdFromToken(jwtToken);
                return string.IsNullOrWhiteSpace(userId) ? null : userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.GetUserIdFromJwt] Error parsing token");
                return null;
            }
        }

        public async Task<string?> GetUserIdAsync(HttpContext httpContext)
        {
            try
            {
                var token = ExtractTokenFromHeader(httpContext);
                if (token == null) return null;
                return await GetUserIdFromJwt(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.GetUserIdAsync] Error");
                return null;
            }
        }

        public async Task<UserInfoDto?> GetCurrentUserAsync(HttpContext httpContext)
        {
            try
            {
                var token = ExtractTokenFromHeader(httpContext);
                if (token == null) return null;
                return await GetCurrentUserFromJwt(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.GetCurrentUserAsync] Error");
                return null;
            }
        }

        private string? ExtractTokenFromHeader(HttpContext httpContext)
        {
            var header = httpContext?.Request?.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(header)) return null;
            if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return null;
            return header.Substring("Bearer ".Length).Trim();
        }

        #endregion

        private string MailBody(string mail, string resetLink)
        {
            return $@"
                <html>
                    <body>
                        <p>Hi {mail},</p>
                        <p>We received a request to reset your password. Click the link below to reset your password:</p>
                        <p><a href='{resetLink}'>Reset Password</a></p>
                        <p>If you did not request a password reset, please ignore this email or contact support if you have questions.</p>
                        <p>Thanks,</p>
                        <p>The Team</p>
                    </body>
                </html>";
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

    }
}
