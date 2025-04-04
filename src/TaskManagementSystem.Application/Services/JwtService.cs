using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class JwtService : IJwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            IConfiguration configuration,
            ILogger<JwtService> logger
            )
        {
            _logger = logger;
            // Try to get from appsettings, then fallback to env var.
            _secretKey = !string.IsNullOrWhiteSpace(configuration["Jwt:SecretKey"]) ? configuration["Jwt:SecretKey"]! : Environment.GetEnvironmentVariable("TskMgr_Jwt__SecretKey")!;
            _issuer = !string.IsNullOrWhiteSpace(configuration["Jwt:Issuer"]) ? configuration["Jwt:Issuer"]! : Environment.GetEnvironmentVariable("TskMgr_Jwt__Issuer")!;
            _audience = !string.IsNullOrWhiteSpace(configuration["Jwt:Audience"]) ? configuration["Jwt:Audience"]! : Environment.GetEnvironmentVariable("TskMgr_Jwt__Audience")!;
            ValidateJwtSettings();
        }

        private void ValidateJwtSettings()
        {
            if (string.IsNullOrWhiteSpace(_secretKey))
            {
                throw new ArgumentNullException(nameof(_secretKey), "JWT SecretKey is not set.");
            }
            if (string.IsNullOrWhiteSpace(_issuer))
            {
                throw new ArgumentNullException(nameof(_issuer), "JWT Issuer is not set.");
            }
            if (string.IsNullOrWhiteSpace(_audience))
            {
                throw new ArgumentNullException(nameof(_audience), "JWT Audience is not set.");
            }
        }

        public TokenResponse GenerateJwtToken(User user, List<string> roles = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Add roles to claims
            if (roles?.Any() ?? false)
            {
                foreach (var role in roles)
                {
                    claims = claims.Append(new Claim(ClaimTypes.Role, role)).ToArray();
                }
            }

            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), // Short-lived access token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer,
                Audience = _audience
            };
            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);

            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    }),
                Expires = DateTime.UtcNow.AddDays(7), // Long-lived refresh token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer,
                Audience = _audience
            };
            var refreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);

            return new TokenResponse
            {
                AccessToken = tokenHandler.WriteToken(accessToken),
                RefreshToken = tokenHandler.WriteToken(refreshToken),
                Expiration = ToUnixTimestamp(accessTokenDescriptor.Expires.Value)
            };
        }

        private long ToUnixTimestamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = false
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                if (securityToken is JwtSecurityToken jwtSecurityToken &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }
            catch (SecurityTokenException ex)
            {
                // Token validation failed (invalid signature, issuer, audience, etc.)
                // Log the exception for debugging purposes.
                _logger.LogError(ex, "Token validation failed: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                // Handle any other unexpected exceptions during token processing.
                _logger.LogError(ex, "Error processing token: {Message}", ex.Message);
                return null;
            }

            return null;
        }
    }

    public interface IJwtService
    {
        TokenResponse GenerateJwtToken(User user, List<string> roles = null);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
