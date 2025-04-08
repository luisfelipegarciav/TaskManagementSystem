using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto userDto);
        Task<ServiceResponse<User>> GetByUsernameAsync(string username);
        Task<ServiceResponse<IEnumerable<Role>>> GetByRolesByUserId(int userId);
        Task<ServiceResponse<User>> GetByIdAsync(int it);
        Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordRequestDto changePasswordRequest);
        Task<ServiceResponse<bool>> DeleteUserByIdAsync(int userId);
        Task<ServiceResponse<bool>> UpdateUserAsync(UpdateUserDto dto);
    }
}
