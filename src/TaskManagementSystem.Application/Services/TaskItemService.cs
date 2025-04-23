using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ILogger<TaskItemService> _logger;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly ICategoryService _categoryService;
        private readonly IRepository<TaskItem> _genericRepository;

        public TaskItemService(
            ILogger<TaskItemService> logger,
            ITaskItemRepository taskItemRepository,
            ICategoryService categoryService,
            IRepository<TaskItem> genericRepository
            )
        {
            _logger = logger;
            _taskItemRepository = taskItemRepository;
            _categoryService = categoryService;
            _genericRepository = genericRepository;
        }

        public async Task<ServiceResponse<TaskItemDto>> CreateTaskItemAsync(int userId, CreateTaskItemDto taskItemDto)
        {
            try
            {
                if (userId < 1)
                    throw new InvalidModelException("Invalid user found.");

                await ValidateTaskItem(taskItemDto);

                Enum.TryParse(taskItemDto.Priority, out Priority priority);

                var entity = new TaskItem()
                {
                    Title = taskItemDto.Title,
                    Description = taskItemDto.Description,
                    DueDate = taskItemDto.DueDate,
                    Priority = priority,
                    CategoryId = taskItemDto.CategoryId,
                    UserId = userId
                };

                var id = await _genericRepository.AddAsync(entity);

                return ServiceResponse<TaskItemDto>.Success(new TaskItemDto
                {
                    Id = entity.Id
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task.");
                return ServiceResponse<TaskItemDto>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTaskItemAsync(int id, int userId)
        {
            try
            {
                if (userId < 1)
                    throw new InvalidModelException("Invalid user found.");

                var currentTaskItem = (await GetTaskItemByIdAsync(id, userId)).Data;

                await _genericRepository.DeleteAsync(currentTaskItem.Id);

                return ServiceResponse<bool>.Success(true);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task item.");
                return ServiceResponse<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<TaskItemDto>> GetTaskItemByIdAsync(int id, int userId)
        {
            try
            {
                if (id < 1)
                    throw new InvalidModelException("Invalid task id.");

                if (userId < 1)
                    throw new InvalidModelException("Invalid user found.");

                var currentTaskItem = await _genericRepository.GetByIdAsync(id);
                if (currentTaskItem == null)
                {
                    throw new TaskItemNotFoundException();
                } else if (currentTaskItem.UserId != userId)
                {
                    throw new TaskItemNotFoundException("Task item do not belongs to current user");
                }


                var getCategoryByIdResponse = await _categoryService.GetCategoryByIdAsync(currentTaskItem.CategoryId);
                if (!getCategoryByIdResponse.IsSuccessful)
                {
                    throw new CategoryNotFoundException();
                }

                var category = getCategoryByIdResponse.Data;

                return ServiceResponse<TaskItemDto>.Success(new TaskItemDto
                {
                    Id = currentTaskItem.Id,
                    Title = currentTaskItem.Title,
                    Description = currentTaskItem.Description,
                    DueDate = currentTaskItem.DueDate,
                    Priority = (Priority)Enum.Parse(typeof(Priority), currentTaskItem.Priority.ToString()),
                    CategoryId = currentTaskItem.CategoryId,
                    CategoryName = category.Name,
                    CreatedAt = currentTaskItem.CreatedAt,
                    IsCompleted = currentTaskItem.Completed,
                    CompletedAt = currentTaskItem.CompletedAt
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task item.");
                return ServiceResponse<TaskItemDto>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<PaginatedResultDto<TaskItemDto>>> GetTaskItemsByUserIdAsync(int id, PaginationParamsDto paginationParams)
        {
            try
            {
                if (id < 1)
                    throw new InvalidModelException("Invalid user id.");

                if (paginationParams == null
                    || paginationParams.PageNumber < 1
                    || paginationParams.PageSize < 1)
                    throw new InvalidModelException("Invalid pagination params.");

                var taskCount = (await GetTaskItemsCountByUserIdAsync(id)).Data;

                var items = new List<TaskItemDto>();
                if (taskCount > 0)
                {
                    var categories = (await _categoryService.GetAllCategoriesAsync()).Data;

                    var rowOffset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;
                    var tasks = await _taskItemRepository.GetTaskItemsByUserIdAsync(id, rowOffset, paginationParams.PageSize);
                    items = tasks?.Select(x =>
                    new TaskItemDto
                    {
                        CategoryId = x.CategoryId,
                        Title = x.Title,
                        Description = x.Description,
                        DueDate = x.DueDate,
                        Priority = x.Priority,
                        Id = x.Id,
                        IsCompleted = x.Completed,
                        CreatedAt = x.CreatedAt,
                        CompletedAt = x.CompletedAt,
                        CategoryName = categories?.FirstOrDefault(c => c.Id == x.CategoryId)?.Name
                    })?.ToList();
                }

                return ServiceResponse<PaginatedResultDto<TaskItemDto>>.Success(new PaginatedResultDto<TaskItemDto>
                {
                    Items = items ?? new List<TaskItemDto>(),
                    TotalCount = taskCount,
                    PageNumber = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task items by user id.");
                return ServiceResponse<PaginatedResultDto<TaskItemDto>>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> MarkTaskItemAsCompletedAsync(int id, int userId)
        {
            try
            {
                if (id < 1)
                    throw new InvalidModelException("Invalid task id.");

                if (userId < 1)
                    throw new InvalidModelException("Invalid user.");

                var currentTaskItem = (await GetTaskItemByIdAsync(id, userId)).Data;

                if (currentTaskItem.IsCompleted)
                    throw new UpdateEntityException("Task already mark as completed.");

                await _taskItemRepository.MarkTaskItemAsCompletedByIdAsync(id, DateTime.UtcNow);

                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking task as completed.");
                return ServiceResponse<bool>.Failure(ex.Message);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateTaskItemAsync(int id, int userId, UpdateTaskItemDto taskItemDto)
        {
            try
            {
                await ValidateTaskItem(taskItemDto);

                var currentTaskItem = (await GetTaskItemByIdAsync(id, userId)).Data;
                if (currentTaskItem.IsCompleted)
                {
                    throw new UpdateEntityException("Task item already completed, can not be updated.");
                }

                Enum.TryParse(taskItemDto.Priority, out Priority priority);

                var entity = new TaskItem()
                {
                    Id = id,
                    Title = taskItemDto.Title,
                    Description = taskItemDto.Description,
                    DueDate = taskItemDto.DueDate,
                    Priority = priority,
                    CategoryId = taskItemDto.CategoryId,
                    UserId = userId
                };

                await _genericRepository.UpdateAsync(entity);

                return ServiceResponse<bool>.Success(true);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task item.");
                return ServiceResponse<bool>.Failure(ex.Message);
            }
        }

        private bool IsValidPriority(string priority)
        {
            return Enum.TryParse(priority, out Priority _);
        }

        private bool IsValidDueDate(DateTime dueDate)
        {
            return dueDate.Date >= DateTime.UtcNow.Date;
        }

        private async Task ValidateTaskItem(CreateTaskItemDto taskItemDto)
        {
            if (taskItemDto == null)
            {
                throw new InvalidModelException("Unable to process data.");
            }

            if (string.IsNullOrWhiteSpace(taskItemDto.Title))
                throw new InvalidModelException("Title is required.");

            if (string.IsNullOrWhiteSpace(taskItemDto.Description))
                throw new InvalidModelException("Description is required.");

            if (string.IsNullOrWhiteSpace(taskItemDto.Priority))
                throw new InvalidModelException("Priority is required.");

            if (taskItemDto.CategoryId < 1)
                throw new InvalidModelException("CategoryId is required.");

            if (!IsValidPriority(taskItemDto.Priority.ToString()))
                throw new InvalidModelException("Invalid priority.");

            if (!IsValidDueDate(taskItemDto.DueDate))
                throw new InvalidModelException("Invalid Due date.");

            var getCategoryByIdResponse = await _categoryService.GetCategoryByIdAsync(taskItemDto.CategoryId);
            if (!getCategoryByIdResponse.IsSuccessful)
            {
                throw new CategoryNotFoundException();
            }
        }

        public async Task<ServiceResponse<int>> GetTaskItemsCountByUserIdAsync(int userId)
        {
            try
            {
                if (userId < 1)
                    throw new InvalidModelException("Invalid user.");

                var taskCount = await _taskItemRepository.GetTaskItemsCountByUserIdAsync(userId);

                return ServiceResponse<int>.Success(taskCount);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task items count by user id.");
                return ServiceResponse<int>.Failure(ex.Message);
            }
        }
    }
}
