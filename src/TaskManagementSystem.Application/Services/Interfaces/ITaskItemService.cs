namespace TaskManagementSystem.Application
{
    public interface ITaskItemService
    {
        Task<ServiceResponse<TaskItemDto>> CreateTaskItemAsync(int userId, CreateTaskItemDto taskItemDto);
        //Task<ServiceResponse<bool>> UpdateTaskItemAsync(int id, UpdateTaskItemDto taskItemDto);
        //Task<ServiceResponse<bool>> DeleteTaskItemAsync(int id);
        //Task<ServiceResponse<IEnumerable<TaskItemDto>>> GetAllTaskItemsAsync(int userId);
        Task<ServiceResponse<TaskItemDto>> GetTaskItemByIdAsync(int id, int userId);
        Task<ServiceResponse<PaginatedResultDto<TaskItemDto>>> GetTaskItemsByUserIdAsync(int id, PaginationParamsDto paginationParams);
    }
}
