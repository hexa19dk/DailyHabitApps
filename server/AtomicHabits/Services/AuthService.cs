using AtomicHabits.Models;
using AtomicHabits.Models.DTO;
using AtomicHabits.Repositories;
using AtomicHabits.Service;
using Azure;
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
        Task<ApiResponse> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDTO dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default);
        Task<ApiResponse> StoreRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default);
        Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);


        Task<UserInfoDto?> GetCurrentUserFromJwt(string? jwtToken);
        Task<string?> GetUserIdFromJwt(string? jwtToken);

        Task<string?> GetUserIdAsync(HttpContext httpContext);
        Task<UserInfoDto?> GetCurrentUserAsync(HttpContext httpContext);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _tokenService;
        private readonly IUserRepositories _userRepo;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthService> _logger;
        private ApiResponse _response;

        public AuthService(AppDbContext db, ITokenService tokenService, IUserRepositories userRepo, IEmailSender emailSender, ILogger<AuthService> logger)
        {
            _db = db;
            _tokenService = tokenService;
            _userRepo = userRepo;
            _emailSender = emailSender;
            _logger = logger;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
        {
            var response = new ApiResponse();
            using var trx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (await _db.Users.AnyAsync(x => x.Username == dto.Username || x.Email == dto.Email, cancellationToken))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Username or Email already exists");
                    return response;
                }

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _db.Users.AddAsync(user, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);

                // assign role if exists
                var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role, cancellationToken);
                if (role != null)
                {
                    await _db.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id }, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }

                // generate tokens
                var roles = await _db.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Include(ur => ur.Role)
                    .Select(ur => ur.Role.Name)
                    .ToListAsync(cancellationToken);

                var accessToken = await _tokenService.GenerateToken(user, roles);
                var refreshToken = await _tokenService.GenerateRefreshToken(user);
                await StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);

                await trx.CommitAsync(cancellationToken);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = new
                {
                    token = accessToken,
                    refreshToken = refreshToken,
                    itemuser = new { Username = dto.Username, Email = dto.Email }
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.RegisterAsync] Error for {Username}", dto?.Username);
                await trx.RollbackAsync(cancellationToken);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<ApiResponse> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _db.Users
                    .Include(u => u.UserRoles)!
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Username, cancellationToken);

                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Invalid Credentials");
                    return response;
                }

                var roles = user.UserRoles!.Select(u => u.Role.Name).ToList();
                var accessToken = await _tokenService.GenerateToken(user, roles);
                var refreshToken = await _tokenService.GenerateRefreshToken(user);

                await StoreRefreshTokenAsync(user.Id, refreshToken, cancellationToken);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = new
                {
                    accessToken,
                    refreshToken,
                    expiresAt = DateTime.UtcNow.AddMinutes(15)
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.LoginAsync] Error login attempt for {Username}", dto?.Username);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
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
            var response = new ApiResponse();

            try
            {
                var user = await _userRepo.GetByEmailAsync(dto.Email);
                if (user == null)
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("User not found.");
                    return response;
                }

                var token = await GeneratePasswordResetTokenAsync(dto.Email);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var resetPath = "http://localhost:3000/ResetPassword?" + $"token={token}&" + $"email={user.Email}";

                await _emailSender.SendEmailAsync(dto.Email, "Reset Password", MailBody(dto.Email, HtmlEncoder.Default.Encode(resetPath)));

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = new ForgotPasswordDTO();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.ForgotPasswordAsync] Error for {Email}", dto.Email);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDTO dto, CancellationToken cancellationToken = default)
        {
            var response = new ApiResponse();

            //if (!dto.IsValid())
            //{
            //    response.IsSuccess = false;
            //    response.StatusCode = HttpStatusCode.BadRequest;
            //    response.Result = "Invalid payload";
            //    return response;
            //}

            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("User not found.");
                return response;
            }

            var decodedToken = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
            if (user.PasswordResetToken != decodedToken || user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.BadRequest;
                response.ErrorMessages.Add("Invalid or expired reset token.");
                return response;
            }

            var setToken = await _userRepo.SetPasswordResetTokenAsync(user, decodedToken);
            if (!setToken)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Failed to set reset token state.");
                return response;
            }

            var updateResult = await _userRepo.UpdatePasswordAsync(user, dto.Password);
            if (!updateResult)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Failed to update password");
                return response;
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return response;
        }

        public async Task<ApiResponse> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
        {
            var response = new ApiResponse();
            try
            {
                if (string.IsNullOrEmpty(dto.RefreshToken))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Refresh token is required");
                    return response;
                }

                var userIdStr = _tokenService.GetUserIdFromToken(dto.RefreshToken);
                if (string.IsNullOrEmpty(userIdStr))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Invalid refresh token");
                    return response;
                }

                if (!int.TryParse(userIdStr, out var userId))
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Invalid refresh token payload");
                    return response;
                }

                var user = await _db.Users
                    .Include(u => u.UserRoles)!
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("User not found");
                    return response;
                }

                if (user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.IsSuccess = false;
                    response.ErrorMessages.Add("Invalid or expired refresh token");
                    return response;
                }

                var roles = user.UserRoles!.Select(u => u.Role.Name).ToList();
                var newAccessToken = await _tokenService.GenerateToken(user, roles);
                var newRefreshToken = await _tokenService.GenerateRefreshToken(user);

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                _db.Users.Update(user);
                await _db.SaveChangesAsync(cancellationToken);

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken,
                    expiresAt = DateTime.UtcNow.AddMinutes(15)
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.RefreshTokenAsync] Error");
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<ApiResponse> StoreRefreshTokenAsync(int userId, string refreshToken, CancellationToken cancellationToken = default)
        {
            var response = new ApiResponse();
            try
            {
                var user = await _db.Users.FindAsync(new object[] { userId }, cancellationToken);
                if (user != null)
                {
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(3);
                    await _db.SaveChangesAsync(cancellationToken);

                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    response.Result = new
                    {
                        user.RefreshToken, user.RefreshTokenExpiry
                    };

                    return response;
                }

                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add("User not found, please get any user that exists.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthService.StoreRefreshTokenAsync] Error storing refresh token for {UserId}", userId);

                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages = new List<string>() { ex.Message.ToString() };
                return response;
            }
        }

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



        public async Task<UserInfoDto?> GetCurrentUserFromJwt(string? jwtToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jwtToken)) return null;
                var claims = _tokenService.GetClaimsFromToken(jwtToken);
                if (claims == null || claims.Count == 0) return null;

                var userId = claims.ContainsKey("userId") ? claims["userId"] : null;
                var email = claims.ContainsKey("email") ? claims["email"] : null;
                var username = claims.ContainsKey("username") ? claims["username"] : null;

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


    }
}
