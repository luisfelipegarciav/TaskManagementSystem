using Microsoft.Extensions.Logging;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class TaskItemService: ITaskItemService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly ICategoryService _categoryService;

        public TaskItemService(
            ILogger<CategoryService> logger,
            ITaskItemRepository taskItemRepository,
            ICategoryService categoryService
            )
        {
            _logger = logger;
            _taskItemRepository = taskItemRepository;
            _categoryService = categoryService;
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
    }
}
