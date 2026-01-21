using AtomicHabits.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AtomicHabits.Service
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user, List<string> roles);
        Task<string> GenerateRefreshToken();
        string GetUserIdFromToken(string token);
        Dictionary<string, string>? GetClaimsFromToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly ILogger<TokenService> _log;

        public TokenService(IConfiguration config, ILogger<TokenService> log) 
        {
            _config = config;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!));
            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
            _log = log;
        }

        public Task<string> GenerateToken(User user, List<string> roles)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new("username", user.Username ?? string.Empty)
                };

                roles.ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r)));               

                var token = new JwtSecurityToken(
                    _issuer,
                    _audience,
                    claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
                );

                return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                _log.LogError("Genereate Token error: " + ex.Message);
                throw new Exception("Genereate Token error: " + ex.Message);
            }           
        }

        public Task<string> GenerateRefreshToken()
        {
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return Task.FromResult(rawToken);
        }

        public string GetUserIdFromToken(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        }

        public Dictionary<string, string>? GetClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = _signingKey
            };


            var principal = handler.ValidateToken(token, parameters, out _);
            return principal.Claims.ToDictionary(c => c.Type, c => c.Value);
        }
    }
}
