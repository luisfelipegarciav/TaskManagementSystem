using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto userDto);
        Task<ServiceResponse<User>> GetByUsernameAsync(string username);
    }
}
