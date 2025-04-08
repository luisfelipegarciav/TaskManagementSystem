using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRespositoryGeneric;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ITaskItemService _taskItemService;

        public UserService(
            ILogger<UserService> logger,
            IRepository<User> userRespositoryGeneric,
            IPasswordHasher<User> passwordHasher,
            IUserRepository userRepository,
            ITaskItemService taskItemService
            )
        {
            _logger = logger;
            _userRespositoryGeneric = userRespositoryGeneric;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _taskItemService = taskItemService;
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordRequest)
        {
            try
            {
                if (changePasswordRequest == null
                    || string.IsNullOrWhiteSpace(changePasswordRequest.CurrentPassword)
                    || string.IsNullOrWhiteSpace(changePasswordRequest.NewPassword)
                    || changePasswordRequest.NewPassword.Equals(changePasswordRequest.CurrentPassword))
                    throw new InvalidModelException();

                var currentUser = (await GetByIdAsync(userId)).Data;
                if (currentUser == null)
                    throw new EntityNotFoundException("User not found.");

                var result = _passwordHasher.VerifyHashedPassword(currentUser, currentUser.PasswordHash, changePasswordRequest.CurrentPassword);
                if (result != PasswordVerificationResult.Success)
                    throw new EntityNotFoundException("User not found.");

                currentUser.PasswordHash = _passwordHasher.HashPassword(currentUser, changePasswordRequest.NewPassword);

                await _userRepository.ChangePasswordAsync(currentUser.Id, currentUser.PasswordHash);

                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UserService)}.{nameof(ChangePasswordAsync)}");
                return ServiceResponse<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto userDto)
        {
            try
            {
                if (userDto == null)
                    throw new InvalidModelException("Invalid user data.");

                if (string.IsNullOrWhiteSpace(userDto.Username))
                    throw new InvalidModelException("Invalid username");

                if (string.IsNullOrWhiteSpace(userDto.Email))
                    throw new InvalidModelException("Invalid email");

                if (string.IsNullOrWhiteSpace(userDto.Password))
                    throw new InvalidModelException("Invalid password");

                if (string.IsNullOrWhiteSpace(userDto.Username))
                    throw new InvalidModelException("Invalid username");

                if (userDto.Roles == null || userDto.Roles.Count == 0)
                    throw new InvalidModelException("Roles are required.");

                var roles = new List<int>();
                foreach (var role in userDto.Roles.Select(x => x).Distinct())
                {
                    var roleId = await _userRepository.GetRoleByNameAsync(role);
                    if (roleId == null)
                        throw new InvalidModelException($"Role {role} not found.");
                    roles.Add(roleId.Id);
                }

                var user = new User
                {
                    Email = userDto.Email,
                    Username = userDto.Username
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

                var createdUser = await _userRespositoryGeneric.AddAsync(user);

                if (createdUser == null)
                    throw new InvalidModelException("User not created.");

                foreach (var roleId in roles)
                {
                    var userRole = new UserRole
                    {
                        UserId = createdUser.Id,
                        RoleId = roleId
                    };
                    await _userRepository.AddUserRoleAsync(userRole);
                }

                return ServiceResponse<UserDto>.Success(new UserDto
                {
                    Id = createdUser.Id,
                    Email = userDto.Email,
                    Username = userDto.Username
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user.");
                return ServiceResponse<UserDto>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteUserByIdAsync(int userId)
        {
            try
            {
                if (userId < 1)
                    throw new InvalidModelException("Invalid user id.");

                var getTaskItemsCountByUserIdResponse = await _taskItemService.GetTaskItemsCountByUserIdAsync(userId);
                if (getTaskItemsCountByUserIdResponse.IsSuccessful && getTaskItemsCountByUserIdResponse.Data > 0)
                    return ServiceResponse<bool>.Failure("User cannot be deleted because it has task items.");

                await _userRepository.DeleteUserRolesByIdAsync(userId);

                await _userRespositoryGeneric.DeleteAsync(userId);
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UserService)}.{nameof(DeleteUserByIdAsync)}");
                return ServiceResponse<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<User>> GetByIdAsync(int it)
        {
            try
            {
                var result = await _userRespositoryGeneric.GetByIdAsync(it);
                return ServiceResponse<User>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UserService)}.{nameof(GetByUsernameAsync)}");
                return ServiceResponse<User>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<IEnumerable<Role>>> GetByRolesByUserId(int userId)
        {
            try
            {
                var result = await _userRepository.GetUserRolesAsync(userId);
                return ServiceResponse<IEnumerable<Role>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UserService)}.{nameof(GetByUsernameAsync)}");
                return ServiceResponse<IEnumerable<Role>>.Failure(ex.Message);
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
