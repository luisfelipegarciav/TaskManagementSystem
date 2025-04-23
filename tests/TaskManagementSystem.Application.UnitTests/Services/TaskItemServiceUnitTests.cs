using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application.UnitTests.Services
{
    public class TaskItemServiceUnitTests
    {
        private readonly Mock<ILogger<TaskItemService>> _mockLogger;
        private readonly Mock<ITaskItemRepository> _mockTaskItemRepository;
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly Mock<IRepository<TaskItem>> _mockTaskItemRepositoryGeneric;
        private readonly TaskItemService _taskItemService;

        public TaskItemServiceUnitTests()
        {
            _mockLogger = new Mock<ILogger<TaskItemService>>();
            _mockCategoryService = new Mock<ICategoryService>();
            _mockTaskItemRepository = new Mock<ITaskItemRepository>();
            _mockTaskItemRepositoryGeneric = new Mock<IRepository<TaskItem>>();
            _taskItemService = new TaskItemService(
                _mockLogger.Object,
                _mockTaskItemRepository.Object,
                _mockCategoryService.Object,
                _mockTaskItemRepositoryGeneric.Object);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_InvalidUser()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Invalid user found.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 0;
            var taskItem = new CreateTaskItemDto();

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_UnableToProcessData()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Unable to process data.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("UnableToProcessData");

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_TitleRequired()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Title is required.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("TitleRequired");

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_DescriptionRequired()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Description is required.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("DescriptionRequired");

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_PriorityRequired()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Priority is required.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("PriorityRequired");

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_CategoryIdRequired()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "CategoryId is required.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("CategoryIdRequired");

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_InvalidPriority()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Invalid priority.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("InvalidPriority");

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_InvalidDueDate()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Invalid Due date.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("InvalidDueDate");

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Failure_CategoryNotFound()
        {
            // Arrange
            var expectedLogMessage = "Error creating task";
            var expectedExceptionMessage = "Category not found";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            var taskItem = SetupTaskItem("CategoryNotFound");

            _mockCategoryService.Setup(x => x.GetCategoryByIdAsync(taskItem.CategoryId))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Failure(expectedExceptionMessage));

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockCategoryService.Verify(x => x.GetCategoryByIdAsync(taskItem.CategoryId), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_CreateTaskItemAsync_Success()
        {
            // Arrange
            var userId = 1;
            var taskItem = SetupTaskItem("Success");

            _mockCategoryService.Setup(x => x.GetCategoryByIdAsync(taskItem.CategoryId))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Success(data: new CategoryDto { Id = 1, Name = "abc" }));

            // Act
            var result = await _taskItemService.CreateTaskItemAsync(userId, taskItem);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            _mockCategoryService.Verify(x => x.GetCategoryByIdAsync(taskItem.CategoryId), times: Times.AtLeastOnce);
        }

        private CreateTaskItemDto SetupTaskItem(string testCase)
        {
            CreateTaskItemDto taskItemDto = new CreateTaskItemDto()
            {
                CategoryId = 1,
                Description = "Test Description",
                Title = "Test Title",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "High"
            };

            switch (testCase)
            {
                case "TitleRequired":
                    taskItemDto.Title = null;
                    break;
                case "DescriptionRequired":
                    taskItemDto.Description = null;
                    break;
                case "PriorityRequired":
                    taskItemDto.Priority = null;
                    break;
                case "CategoryIdRequired":
                    taskItemDto.CategoryId = 0;
                    break;
                case "InvalidPriority":
                    taskItemDto.Priority = "qwerty";
                    break;
                case "InvalidDueDate":
                    taskItemDto.DueDate = DateTime.UtcNow.AddDays(-2);
                    break;
                case "CategoryNotFound":
                    break;
                case "UnableToProcessData":
                    taskItemDto = null;
                    break;
                default:
                    break;
            }

            return taskItemDto;
        }

        [Fact]
        public async Task Handle_TaskItemService_DeleteTaskItemAsync_Failure_InvalidUser()
        {
            // Arrange
            var expectedLogMessage = "Error deleting task item.";
            var expectedExceptionMessage = "Invalid user found.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 0;

            // Act
            var result = await _taskItemService.DeleteTaskItemAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_DeleteTaskItemAsync_Success()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;

            var taskItem = SetupTaskItem(taskId, userId);

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(taskItem);

            _mockCategoryService
                .Setup(x => x.GetCategoryByIdAsync(taskItem.CategoryId))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Success(data: new CategoryDto { Id = 1, Name = "abc" }));

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.DeleteAsync(taskId));

            // Act
            var result = await _taskItemService.DeleteTaskItemAsync(taskId, userId);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.GetByIdAsync(taskId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetCategoryByIdAsync(taskItem.CategoryId), times: Times.AtLeastOnce);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.DeleteAsync(taskId), times: Times.AtLeastOnce);
        }

        private TaskItem SetupTaskItem(int taskId, int userId)
        {
            return new TaskItem
            {
                Id = taskId,
                UserId = userId,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = Priority.High,
                Completed = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemByIdAsync_Failure_InvalidTaskId()
        {
            // Arrange
            var expectedLogMessage = "Error getting task item.";
            var expectedExceptionMessage = "Invalid task id.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 0;
            var userId = 0;

            // Act
            var result = await _taskItemService.GetTaskItemByIdAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemByIdAsync_Failure_InvalidUser()
        {
            // Arrange
            var expectedLogMessage = "Error getting task item.";
            var expectedExceptionMessage = "Invalid user found.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 0;

            // Act
            var result = await _taskItemService.GetTaskItemByIdAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemByIdAsync_Failure_TaskItemNotFound()
        {
            // Arrange
            var expectedLogMessage = "Error getting task item.";
            var expectedExceptionMessage = "Task Item not found";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 1;

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync((TaskItem)null);

            // Act
            var result = await _taskItemService.GetTaskItemByIdAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemByIdAsync_Failure_TaskItemDoNotBelongsToUser()
        {
            // Arrange
            var expectedLogMessage = "Error getting task item.";
            var expectedExceptionMessage = "Task item do not belongs to current user";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 1;

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(new TaskItem
                { Description = "abc", UserId = 2, Title= "qwerty" });

            // Act
            var result = await _taskItemService.GetTaskItemByIdAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemByIdAsync_Failure_CategoryNotFound()
        {
            // Arrange
            var expectedLogMessage = "Error getting task item.";
            var expectedExceptionMessage = "Category not found";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 1;

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(new TaskItem
                { Description = "abc", UserId = userId, Title = "qwerty", CategoryId = 2 });

            _mockCategoryService
                .Setup(x => x.GetCategoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Failure(expectedExceptionMessage));

            // Act
            var result = await _taskItemService.GetTaskItemByIdAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.GetByIdAsync(taskId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetCategoryByIdAsync(It.IsAny<int>()), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemByIdAsync_Success()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(new TaskItem
                { Description = "abc", UserId = userId, Title = "qwerty", CategoryId = 2 });

            _mockCategoryService
                .Setup(x => x.GetCategoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Success(new CategoryDto { Id = 1, Name = "abc" }));

            // Act
            var result = await _taskItemService.GetTaskItemByIdAsync(taskId, userId);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.GetByIdAsync(taskId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetCategoryByIdAsync(It.IsAny<int>()), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemsByUserIdAsync_Failure_InvalidUser()
        {
            // Arrange
            var expectedLogMessage = "Error getting task items by user id.";
            var expectedExceptionMessage = "Invalid user id.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);
            
            var userId = 0;
            var paginationParams = new PaginationParamsDto();

            // Act
            var result = await _taskItemService.GetTaskItemsByUserIdAsync(userId, paginationParams);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemsByUserIdAsync_Failure_InvalidPagination()
        {
            // Arrange
            var expectedLogMessage = "Error getting task items by user id.";
            var expectedExceptionMessage = "Invalid pagination params.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 1;
            PaginationParamsDto paginationParams = null;

            // Act
            var result = await _taskItemService.GetTaskItemsByUserIdAsync(userId, paginationParams);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemsByUserIdAsync_Success()
        {
            // Arrange
            var userId = 1;
            var paginationParams = new PaginationParamsDto();
            paginationParams.PageNumber = 1;
            var rowsOffset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;

            _mockTaskItemRepository
                .Setup(x => x.GetTaskItemsCountByUserIdAsync(userId))
                .ReturnsAsync(1);

            _mockCategoryService
                .Setup(x => x.GetAllCategoriesAsync())
                .ReturnsAsync(ServiceResponse<List<CategoryDto>>.Success(new List<CategoryDto>()
                {
                    new CategoryDto
                    {
                        Id = 1,
                        Name = "Category 1"
                    }
                }));

            _mockTaskItemRepository
                .Setup(x => x.GetTaskItemsByUserIdAsync(userId, rowsOffset, paginationParams.PageSize))
                .ReturnsAsync(new List<TaskItem>()
                {
                    new TaskItem
                    {
                        Id = 1,
                        UserId = userId,
                        Title = "Test Task",
                        Description = "Test Description",
                        DueDate = DateTime.UtcNow.AddDays(1),
                        Priority = Priority.High,
                        Completed = false,
                        CreatedAt = DateTime.UtcNow
                    }
                });

            // Act
            var result = await _taskItemService.GetTaskItemsByUserIdAsync(userId, paginationParams);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            _mockTaskItemRepository
                .Verify(x => x.GetTaskItemsCountByUserIdAsync(userId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetAllCategoriesAsync(), times: Times.AtLeastOnce);

            _mockTaskItemRepository
                .Verify(x => x.GetTaskItemsByUserIdAsync(userId, rowsOffset, paginationParams.PageSize), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_TaskItemService_MarkTaskItemAsCompletedAsync_Failure_InvalidTaskId()
        {
            // Arrange
            var expectedLogMessage = "Error marking task as completed.";
            var expectedExceptionMessage = "Invalid task id.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 0;
            var userId = 1;

            // Act
            var result = await _taskItemService.MarkTaskItemAsCompletedAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_MarkTaskItemAsCompletedAsync_Failure_InvalidUser()
        {
            // Arrange
            var expectedLogMessage = "Error marking task as completed.";
            var expectedExceptionMessage = "Invalid user.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 0;

            // Act
            var result = await _taskItemService.MarkTaskItemAsCompletedAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_MarkTaskItemAsCompletedAsync_Failure_TaskCompleted()
        {
            // Arrange
            var expectedLogMessage = "Error marking task as completed.";
            var expectedExceptionMessage = "Task already mark as completed.";
            var expectedException = new UpdateEntityException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 1;

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(new TaskItem
                {
                    Id = taskId,
                    UserId = userId,
                    Title = "Test Task",
                    Description = "Test Description",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Priority = Priority.High,
                    Completed = true,
                    CreatedAt = DateTime.UtcNow
                });

            _mockCategoryService
                .Setup(x => x.GetCategoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Success(new CategoryDto { Id = 1, Name = "abc" }));

            // Act
            var result = await _taskItemService.MarkTaskItemAsCompletedAsync(taskId, userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.GetByIdAsync(taskId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetCategoryByIdAsync(It.IsAny<int>()), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_MarkTaskItemAsCompletedAsync_Success()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(new TaskItem
                {
                    Id = taskId,
                    UserId = userId,
                    Title = "Test Task",
                    Description = "Test Description",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Priority = Priority.High,
                    Completed = false,
                    CreatedAt = DateTime.UtcNow
                });

            _mockCategoryService
                .Setup(x => x.GetCategoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Success(new CategoryDto { Id = 1, Name = "abc" }));

            // Act
            var result = await _taskItemService.MarkTaskItemAsCompletedAsync(taskId, userId);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.GetByIdAsync(taskId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetCategoryByIdAsync(It.IsAny<int>()), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_TaskItemService_UpdateTaskItemAsync_Failure_TaskCompletedCanNotBeUpdated()
        {
            // Arrange
            var expectedLogMessage = "Error updating task item.";
            var expectedExceptionMessage = "Task item already completed, can not be updated.";
            var expectedException = new UpdateEntityException(expectedExceptionMessage);

            var taskId = 1;
            var userId = 1;

            var taskItem = new UpdateTaskItemDto
            {
                CategoryId = 1,
                Description = "abc",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "High",
                Title = "qwerty"
            };

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(new TaskItem
                {
                    Id = taskId,
                    UserId = userId,
                    Title = "Test Task",
                    Description = "Test Description",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Priority = Priority.High,
                    Completed = true,
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = 1
                });

            _mockCategoryService.Setup(x => x.GetCategoryByIdAsync(taskItem.CategoryId))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Success(data: new CategoryDto { Name = "abc", Id = 1 }));

            // Act
            var result = await _taskItemService.UpdateTaskItemAsync(taskId, userId, taskItem);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.GetByIdAsync(taskId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetCategoryByIdAsync(taskItem.CategoryId), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_UpdateTaskItemAsync_Success()
        {
            // Arrange
            var taskId = 1;
            var userId = 1;

            var taskItem = new UpdateTaskItemDto
            {
                CategoryId = 1,
                Description = "abc",
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = "High",
                Title = "qwerty"
            };

            _mockTaskItemRepositoryGeneric
                .Setup(x => x.GetByIdAsync(taskId))
                .ReturnsAsync(new TaskItem
                {
                    Id = taskId,
                    UserId = userId,
                    Title = "Test Task",
                    Description = "Test Description",
                    DueDate = DateTime.UtcNow.AddDays(1),
                    Priority = Priority.High,
                    Completed = false,
                    CreatedAt = DateTime.UtcNow,
                    CategoryId = 1
                });

            _mockCategoryService.Setup(x => x.GetCategoryByIdAsync(taskItem.CategoryId))
                .ReturnsAsync(ServiceResponse<CategoryDto>.Success(data: new CategoryDto { Name = "abc", Id = 1 }));

            // Act
            var result = await _taskItemService.UpdateTaskItemAsync(taskId, userId, taskItem);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _mockTaskItemRepositoryGeneric
                .Verify(x => x.GetByIdAsync(taskId), times: Times.AtLeastOnce);

            _mockCategoryService
                .Verify(x => x.GetCategoryByIdAsync(taskItem.CategoryId), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemsCountByUserIdAsync_Failure_InvalidUser()
        {
            // Arrange
            var expectedLogMessage = "Error getting task items count by user id.";
            var expectedExceptionMessage = "Invalid user.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 0;

            // Act
            var result = await _taskItemService.GetTaskItemsCountByUserIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(0, result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_TaskItemService_GetTaskItemsCountByUserIdAsync_Success()
        {
            // Arrange
            var userId = 1;

            _mockTaskItemRepository
                .Setup(x => x.GetTaskItemsCountByUserIdAsync(userId))
                .ReturnsAsync(1);

            // Act
            var result = await _taskItemService.GetTaskItemsCountByUserIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal(1, result.Data);
            Assert.Null(result.Message);

            _mockTaskItemRepository
                .Verify(x => x.GetTaskItemsCountByUserIdAsync(userId), times: Times.AtLeastOnce);
        }
    }
}
