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

                if (taskItemDto == null
                    || string.IsNullOrWhiteSpace(taskItemDto.Title)
                    || string.IsNullOrWhiteSpace(taskItemDto.Description)
                    || string.IsNullOrWhiteSpace(taskItemDto.Priority)
                    || taskItemDto.CategoryId < 1
                    || taskItemDto.DueDate.Date < DateTime.Now.Date                    
                    )
                {
                    throw new InvalidModelException("Unable to process data.");
                }

                var currentPriority = Enum.TryParse(taskItemDto.Priority, out Priority priority);
                if (!currentPriority)
                {
                    throw new InvalidModelException("Invalid priority.");
                }

                var getCategoryByIdResponse = await _categoryService.GetCategoryByIdAsync(taskItemDto.CategoryId);
                if (!getCategoryByIdResponse.IsSuccessful)
                {
                    throw new CategoryNotFoundException();
                }

                var entity = new TaskItem()
                {
                    Title = taskItemDto.Title,
                    Description = taskItemDto.Description,
                    DueDate = taskItemDto.DueDate,
                    Priority = priority,
                    CategoryId = taskItemDto.CategoryId
                };

                var id = await _taskItemRepository.CreateTaskItemAsync(userId, entity);

                return ServiceResponse<TaskItemDto>.Success(new TaskItemDto
                {
                    Id = entity.Id
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
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
                    throw new InvalidModelException("Invalid task id.");

                if (paginationParams == null
                    || paginationParams.PageNumber < 1
                    || paginationParams.PageSize < 1)
                    throw new InvalidModelException("Invalid pagination params.");

                var taskCount = await _taskItemRepository.GetTaskItemsCountByUserIdAsync(id);

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
    }
}
