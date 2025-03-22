namespace TaskManagementSystem.Domain
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
    }
}
