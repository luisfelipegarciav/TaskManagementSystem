namespace TaskManagementSystem.Domain
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        Task ChangePasswordAsync(int userId, string password);
        Task DeleteUserRolesByIdAsync(int userId);
    }
}
