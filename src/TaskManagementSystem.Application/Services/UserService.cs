using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRespository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ILogger<UserService> logger,
            IRepository<User> userRespository,
            IPasswordHasher<User> passwordHasher
            )
        {
            _logger = logger;
            _userRespository = userRespository;
            _passwordHasher = passwordHasher;
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

                var createdUser = await _userRespository.AddAsync(user);

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
    }
}
