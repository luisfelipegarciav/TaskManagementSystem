using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IUserService userService,
            IJwtService jwtService,
            IPasswordHasher<User> passwordHasher,
            ILogger<AuthenticationService> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<ServiceResponse<TokenResponse>> AuthenticateAsync(string username, string password)
        {
            try
            {
                var getByUsernameResponse = await _userService.GetByUsernameAsync(username);

                if (!getByUsernameResponse.IsSuccessful
                    || getByUsernameResponse.Data == null)
                {
                    _logger.LogWarning($"Authentication failed: User '{username}' not found.");
                    return ServiceResponse<TokenResponse>.Failure("Invalid username or password.");
                }

                var user = getByUsernameResponse.Data;

                if (user.Status != Status.Active)
                {
                    _logger.LogWarning($"Authentication failed: User status is not active.");
                    return ServiceResponse<TokenResponse>.Failure("Invalid user status.");
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (result != PasswordVerificationResult.Success)
                {
                    _logger.LogWarning($"Authentication failed: Invalid password for user '{username}'.");
                    return ServiceResponse<TokenResponse>.Failure("Invalid username or password.");
                }

                var roles = new List<string>();
                var userRoles = await _userService.GetByRolesByUserId(user.Id);
                if (userRoles.IsSuccessful && (userRoles.Data?.Any() ?? false))
                {
                    roles = userRoles.Data.Select(x => x.Name).ToList();
                }

                _logger.LogInformation($"User '{username}' authenticated successfully.");
                var token = _jwtService.GenerateJwtToken(user, roles);
                return ServiceResponse<TokenResponse>.Success(token, "Authentication successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating user '{username}'.");
                return ServiceResponse<TokenResponse>.Failure("An unexpected error occurred during authentication.");
            }
        }

        public async Task<ServiceResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromToken(request.Token);
                if (principal == null)
                    throw new AuthenticationException("Invalid Refresh Token.");

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                    throw new AuthenticationException("Invalid Refresh Token Claims.");

                var expiryClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp);

                if (expiryClaim == null || !long.TryParse(expiryClaim.Value, out var expiryUnix))
                    throw new AuthenticationException("Refresh token has no expiry claim.");

                var expiryDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expiryUnix).UtcDateTime;

                // Check if the refresh token has already expired
                if (DateTime.UtcNow >= expiryDateTimeUtc)
                    throw new AuthenticationException("Refresh token has expired.");

                var getUserByIdResponse = await _userService.GetByIdAsync(userId);
                if (getUserByIdResponse == null || !getUserByIdResponse.IsSuccessful || getUserByIdResponse.Data == null)
                    throw new AuthenticationException("User associated with refresh token not found.");

                var currentUser = getUserByIdResponse.Data;

                var roles = new List<string>();
                var userRoles = await _userService.GetByRolesByUserId(currentUser.Id);
                if (userRoles.IsSuccessful && (userRoles.Data?.Any() ?? false))
                {
                    roles = userRoles.Data.Select(x => x.Name).ToList();
                }

                var token = _jwtService.GenerateJwtToken(currentUser, roles);
                return ServiceResponse<TokenResponse>.Success(token, "Refresh token successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error on refresh token.");
                return ServiceResponse<TokenResponse>.Failure("An unexpected error occurred during authentication.");
            }
        }
    }
}
