namespace TaskManagementSystem.Domain
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        Task ChangePasswordAsync(int userId, string password);
        Task DeleteUserRolesByIdAsync(int userId);
        Task<Role> GetRoleByNameAsync(string name);
        Task AddUserRoleAsync(UserRole userRole);
        Task<int> GetUsersCountAsync();
        Task<IEnumerable<User>> GetUsersAsync(int rowOffset, int pageSize);
    }
}
