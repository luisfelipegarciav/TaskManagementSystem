namespace TaskManagementSystem.Domain
{
    public interface ICategoryRepository
    {
        Task<Category> GetCategoryByName(string name);
    }
}
