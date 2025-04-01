namespace TaskManagementSystem.Domain
{
    public interface ITaskItemRepository
    {
        Task<TaskItem> CreateTaskItemAsync(int userId, TaskItem taskItem);
    }
}
