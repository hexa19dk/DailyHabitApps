using AtomicHabits.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AtomicHabits.Service
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user, List<string> roles);
        Task<string> GenerateRefreshToken(User user);
        //ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GetUserIdFromToken(string token);
        Dictionary<string, string>? GetClaimsFromToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IJwtKeyProvider _keyProvider;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly ILogger<TokenService> _log;

        public TokenService(IConfiguration config, IJwtKeyProvider keyProvider, ILogger<TokenService> log) 
        {
            _config = config;
            _keyProvider = keyProvider;
            _jwtKey = _config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not configured");
            _issuer = _config["Jwt:Issuer"] ?? "AtomicHabit";
            _audience = _config["Jwt:Audience"] ?? "Piccolo";
            _log = log;
        }

        public async Task<string> GenerateToken(User user, List<string> roles)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim("userId", user.Id.ToString()),
                    new Claim("username", user.Username ?? string.Empty),
                    new Claim("email", user.Email ?? string.Empty)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim("role", role));
                }

                //var key = await _keyProvider.GetKeyAsync();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var expires = DateTime.UtcNow.AddDays(1);

                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _log.LogError("Genereate Token error: " + ex.Message);
                throw new Exception("Genereate Token error: " + ex.Message);
            }
           
        }

        public async Task<string> GenerateRefreshToken(User user)
        {
            try
            {
                //var key = await _keyProvider.GetKeyAsync();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var refreshClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("userId", user.Id.ToString()),
                    new Claim("username", user.Username ?? string.Empty)
                };

                var expires = DateTime.UtcNow.AddDays(3);

                var refreshToken = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: refreshClaims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(refreshToken);
            }
            catch (Exception ex)
            {
                _log.LogError("Genereate Refresh Token error: " + ex.Message);
                throw new Exception("Genereate Token error: " + ex.Message);
            }
        }

        //public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        //{
        //    var getCachedKey = await _keyProvider.GetKeyAsync();

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = true,
        //        ValidateIssuer = true,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = _issuer,
        //        ValidAudience = _audience,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
        //        ValidateLifetime = false
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        //    if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
        //        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        throw new SecurityTokenException("Invalid token");
        //    }

        //    return principal;
        //}
        public string GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);
                return jwt.Claims.FirstOrDefault(x => x.Type == "userId")?.Value ?? string.Empty;
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<string, string>? GetClaimsFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtKey);
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = false // allow expired tokens
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out _);

                return principal.Claims.ToDictionary(c => c.Type, c => c.Value);
            }
            catch (Exception ex)
            {
                _log.LogError($"[TokenService] Failed to parse claims: {ex.Message}");
                return null;
            }
        }
    }
}
