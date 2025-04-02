namespace TaskManagementSystem.Domain
{
    public interface ITaskItemRepository
    {
        Task<IEnumerable<TaskItem>> GetTaskItemsByUserIdAsync(int id, int pageNumber, int pageSize);
        Task<int> GetTaskItemsCountByUserIdAsync(int id);
        Task MarkTaskItemAsCompletedByIdAsync(int id, DateTime updatedAt);
    }
}
