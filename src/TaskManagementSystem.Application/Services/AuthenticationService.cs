using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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

                _logger.LogInformation($"User '{username}' authenticated successfully.");
                var token = _jwtService.GenerateJwtToken(user);
                return ServiceResponse<TokenResponse>.Success(token, "Authentication successful.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating user '{username}'.");
                return ServiceResponse<TokenResponse>.Failure("An unexpected error occurred during authentication.");
            }
        }
    }
}
