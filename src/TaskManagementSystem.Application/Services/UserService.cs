using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRespositoryGeneric;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;

        public UserService(
            ILogger<UserService> logger,
            IRepository<User> userRespositoryGeneric,
            IPasswordHasher<User> passwordHasher,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _userRespositoryGeneric = userRespositoryGeneric;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto userDto)
        {
            try
            {
                var user = new User
                {
                    Email = userDto.Email,
                    Username = userDto.Username
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

                var createdUser = await _userRespositoryGeneric.AddAsync(user);

                return ServiceResponse<UserDto>.Success(new UserDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Username = createdUser.Username
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return ServiceResponse<UserDto>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<User>> GetByUsernameAsync(string username)
        {
            try
            {
                var result = await _userRepository.GetByUsernameAsync(username);
                return ServiceResponse<User>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UserService)}.{nameof(GetByUsernameAsync)}");
                return ServiceResponse<User>.Failure(ex.Message);
            }
        }
    }
}
