namespace TaskManagementSystem.Application
{
    public interface IUserService
    {
        Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto userDto);
    }
}
