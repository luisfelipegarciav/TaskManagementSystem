namespace TaskManagementSystem.Domain
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
    }
}
