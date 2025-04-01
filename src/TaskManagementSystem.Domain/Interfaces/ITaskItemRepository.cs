namespace TaskManagementSystem.Domain
{
    public interface ITaskItemRepository
    {
        Task<TaskItem> CreateTaskItemAsync(int userId, TaskItem taskItem);
        Task<IEnumerable<TaskItem>> GetTaskItemsByUserIdAsync(int id, int pageNumber, int pageSize);
        Task<int> GetTaskItemsCountByUserIdAsync(int id);
    }
}
