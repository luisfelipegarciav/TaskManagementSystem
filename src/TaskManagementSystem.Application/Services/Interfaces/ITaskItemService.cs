namespace TaskManagementSystem.Application
{
    public interface ITaskItemService
    {
        Task<ServiceResponse<TaskItemDto>> CreateTaskItemAsync(int userId, CreateTaskItemDto taskItemDto);
        Task<ServiceResponse<bool>> UpdateTaskItemAsync(int id, int userId, UpdateTaskItemDto taskItemDto);
        Task<ServiceResponse<bool>> DeleteTaskItemAsync(int id, int userId);
        Task<ServiceResponse<TaskItemDto>> GetTaskItemByIdAsync(int id, int userId);
        Task<ServiceResponse<PaginatedResultDto<TaskItemDto>>> GetTaskItemsByUserIdAsync(int id, PaginationParamsDto paginationParams);
        Task<ServiceResponse<bool>> MarkTaskItemAsCompletedAsync(int id, int userId);
        Task<ServiceResponse<int>> GetTaskItemsCountByUserIdAsync(int userId);
    }
}
