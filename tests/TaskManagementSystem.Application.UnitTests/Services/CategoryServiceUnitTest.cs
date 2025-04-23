using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application.UnitTests
{
    public class CategoryServiceUnitTest
    {
        private readonly Mock<IRepository<Category>> _categoryRepositoryGenericMock;
        private readonly Mock<ILogger<CategoryService>> _mockLogger;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceUnitTest()
        {
            _categoryRepositoryGenericMock = new Mock<IRepository<Category>>();
            _mockLogger = new Mock<ILogger<CategoryService>>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_categoryRepositoryGenericMock.Object, _mockLogger.Object, _categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_CategoryService_CreateCategoryAsync_InvalidData()
        {
            // Arrange  
            var expectedException = new InvalidModelException("Invalid category data.");
            var expecteLogMessage = $"Error creating category.";
            var expecteErrorMessage = "Invalid category data.";
            var expectedServiceResponse = ServiceResponse<User>.Failure(expecteErrorMessage);

            var invalidCategoryDto = new CreateCategoryDto();

            // Act  
            var result = await _categoryService.CreateCategoryAsync(invalidCategoryDto);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryService_CreateCategoryAsync_CategoryAlreadyExists()
        {
            // Arrange  
            var categoryDto = new CreateCategoryDto() { Name = "abc" };
            var expecteErrorMessage = $"Category {categoryDto.Name} already exists.";
            var expectedException = new EntityAlreadyExistsException(expecteErrorMessage);
            var expecteLogMessage = $"Error creating category.";
            var expectedServiceResponse = ServiceResponse<User>.Failure(expecteErrorMessage);

            _categoryRepositoryMock
                .Setup(x => x.GetCategoryByName(categoryDto.Name))
                .ReturnsAsync(new Category { Name = categoryDto.Name });

            // Act  
            var result = await _categoryService.CreateCategoryAsync(categoryDto);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);

            _categoryRepositoryMock.Verify(x => x.GetCategoryByName(categoryDto.Name), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryService_CreateCategoryAsync_Success()
        {
            // Arrange  
            var categoryDto = new CreateCategoryDto() { Name = "abc" };

            _categoryRepositoryMock
                .Setup(x => x.GetCategoryByName(categoryDto.Name))
                .ReturnsAsync((Category?)null);

            _categoryRepositoryGenericMock
                .Setup(x => x.AddAsync(It.IsAny<Category>()))
                .ReturnsAsync(new Category() { Name = categoryDto.Name, Id = 1 });

            // Act  
            var result = await _categoryService.CreateCategoryAsync(categoryDto);

            // Assert  
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            _categoryRepositoryMock.Verify(x => x.GetCategoryByName(categoryDto.Name), times: Times.AtLeastOnce);

            _categoryRepositoryGenericMock
                .Verify(x => x.AddAsync(It.IsAny<Category>()), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_CategoryService_DeleteCategoryByIdAsync_InvalidData()
        {
            // Arrange  
            var expecteErrorMessage = "Invalid category data.";
            var expectedException = new InvalidModelException(expecteErrorMessage);
            var expecteLogMessage = $"Error updating category.";
            var expectedServiceResponse = ServiceResponse<User>.Failure(expecteErrorMessage);

            var categoryId = 0;

            // Act  
            var result = await _categoryService.DeleteCategoryByIdAsync(categoryId);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryService_DeleteCategoryByIdAsync_CategoryNotFound()
        {
            // Arrange  
            var categoryId = 1;
            var expecteErrorMessage = "Category not found.";
            var expectedException = new CategoryNotFoundException(expecteErrorMessage);
            var expecteLogMessage = $"Error updating category.";
            var expectedServiceResponse = ServiceResponse<User>.Failure(expecteErrorMessage);

            _categoryRepositoryGenericMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act  
            var result = await _categoryService.DeleteCategoryByIdAsync(categoryId);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);

            _categoryRepositoryGenericMock.Verify(x => x.GetByIdAsync(categoryId), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.Is<Exception>(ex => ex.Message.Contains(expectedException.Message)), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CategoryService_DeleteCategoryByIdAsync_Success()
        {
            // Arrange  
            var categoryId = 1;

            _categoryRepositoryGenericMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(new Category { Name = "abc", Id = categoryId });

            // Act  
            var result = await _categoryService.DeleteCategoryByIdAsync(categoryId);

            // Assert  
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _categoryRepositoryGenericMock.Verify(x => x.GetByIdAsync(categoryId), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_CategoryService_GetAllCategoriesAsync_Failure()
        {
            // Arrange
            var expectedLogMessage = $"Error pulling categories.";
            var expectedExceptionMessage = "Database error";
            var expectedException = new Exception(expectedExceptionMessage);

            _categoryRepositoryGenericMock
                .Setup(x => x.GetAllAsync())
                .ThrowsAsync(expectedException);

            // Act  
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _categoryRepositoryGenericMock.Verify(x => x.GetAllAsync(), times: Times.AtLeastOnce);

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
        public async Task Handle_CategoryService_GetAllCategoriesAsync_Success()
        {
            // Arrange

            _categoryRepositoryGenericMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Category> { new Category { Id = 1, Name = "abc" } });

            // Act  
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert  
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);
            Assert.True(result.Data?.Count > 0);

            _categoryRepositoryGenericMock.Verify(x => x.GetAllAsync(), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_CategoryService_GetCategoryByIdAsync_InvalidModel()
        {
            // Arrange
            var expectedLogMessage = "Error pulling category.";
            var expectedExceptionMessage = "Invalid category id.";
            var expectedException = new Exception(expectedExceptionMessage);
            var categoryId = 0;

            // Act  
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

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
        public async Task Handle_CategoryService_GetCategoryByIdAsync_CategoryNotFound()
        {
            // Arrange
            var expectedLogMessage = "Error pulling category.";
            var expectedExceptionMessage = "Category not found.";
            var expectedException = new Exception(expectedExceptionMessage);
            var categoryId = 1;

            _categoryRepositoryGenericMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act  
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _categoryRepositoryGenericMock
                .Verify(x => x.GetByIdAsync(categoryId), times: Times.AtLeastOnce);

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
        public async Task Handle_CategoryService_GetCategoryByIdAsync_Success()
        {
            // Arrange
            var categoryId = 1;

            _categoryRepositoryGenericMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(new Category { Name = "abc", Id = categoryId });

            // Act  
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert  
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);
            Assert.Equal(result.Data.Id, categoryId);

            _categoryRepositoryGenericMock
                .Verify(x => x.GetByIdAsync(categoryId), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_CategoryService_GetCategoryByNameAsync_InvalidModel()
        {
            // Arrange
            var expectedLogMessage = "Error pulling category.";
            var expectedExceptionMessage = "Invalid category name.";
            var expectedException = new Exception(expectedExceptionMessage);
            var categoryName = string.Empty;

            // Act  
            var result = await _categoryService.GetCategoryByNameAsync(categoryName);

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
        public async Task Handle_CategoryService_GetCategoryByNameAsync_CategoryNotFound()
        {
            // Arrange
            var expectedLogMessage = "Error pulling category.";
            var expectedExceptionMessage = "Category not found.";
            var expectedException = new Exception(expectedExceptionMessage);
            var categoryName = "abc";

            _categoryRepositoryMock
                .Setup(x => x.GetCategoryByName(categoryName))
                .ReturnsAsync((Category?)null);

            // Act  
            var result = await _categoryService.GetCategoryByNameAsync(categoryName);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _categoryRepositoryMock
                .Verify(x => x.GetCategoryByName(categoryName), times: Times.AtLeastOnce);

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
        public async Task Handle_CategoryService_GetCategoryByNameAsync_Success()
        {
            // Arrange
            var categoryName = "abc";

            _categoryRepositoryMock
                .Setup(x => x.GetCategoryByName(categoryName))
                .ReturnsAsync(new Category { Name = categoryName, Id = 1 });

            // Act  
            var result = await _categoryService.GetCategoryByNameAsync(categoryName);

            // Assert  
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);
            Assert.Equal(result.Data.Name, categoryName);

            _categoryRepositoryMock
                .Verify(x => x.GetCategoryByName(categoryName), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_CategoryService_UpdateCategoryAsync_InvalidModel()
        {
            // Arrange
            var expectedLogMessage = "Error updating category.";
            var expectedExceptionMessage = "Invalid category data.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);
            var categoryId = 0;
            var categoryDto = new UpdateCategoryDto();

            // Act  
            var result = await _categoryService.UpdateCategoryAsync(categoryId, categoryDto);

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
        public async Task Handle_CategoryService_UpdateCategoryAsync_CategoryNotFound()
        {
            // Arrange
            var expectedLogMessage = "Error updating category.";
            var expectedExceptionMessage = "Category not found.";
            var expectedException = new CategoryNotFoundException(expectedExceptionMessage);
            var categoryId = 1;
            var categoryDto = new UpdateCategoryDto() { Name = "abc" };

            _categoryRepositoryGenericMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act  
            var result = await _categoryService.UpdateCategoryAsync(categoryId, categoryDto);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _categoryRepositoryGenericMock
                .Verify(x => x.GetByIdAsync(categoryId), times: Times.AtLeastOnce);

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
        public async Task Handle_CategoryService_UpdateCategoryAsync_CategoryAlreadyExists()
        {
            // Arrange
            var expectedLogMessage = "Error updating category.";
            var expectedExceptionMessage = "Category already exists.";
            var expectedException = new EntityAlreadyExistsException(expectedExceptionMessage);
            var categoryId = 1;
            var categoryDto = new UpdateCategoryDto() { Name = "abc" };

            _categoryRepositoryGenericMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(new Category { Name = "abc", Id = categoryId });

            _categoryRepositoryMock
                .Setup(x => x.GetCategoryByName(categoryDto.Name))
                .ReturnsAsync(new Category { Name = categoryDto.Name, Id = 2 });

            // Act  
            var result = await _categoryService.UpdateCategoryAsync(categoryId, categoryDto);

            // Assert  
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _categoryRepositoryGenericMock
                .Verify(x => x.GetByIdAsync(categoryId), times: Times.AtLeastOnce);

            _categoryRepositoryMock
                .Verify(x => x.GetCategoryByName(categoryDto.Name), times: Times.AtLeastOnce);

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
        public async Task Handle_CategoryService_UpdateCategoryAsync_Success()
        {
            // Arrange
            var categoryId = 1;
            var categoryDto = new UpdateCategoryDto() { Name = "abc" };

            _categoryRepositoryGenericMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(new Category { Name = "abc", Id = categoryId });

            _categoryRepositoryMock
                .Setup(x => x.GetCategoryByName(categoryDto.Name))
                .ReturnsAsync((Category?)null);

            // Act  
            var result = await _categoryService.UpdateCategoryAsync(categoryId, categoryDto);

            // Assert  
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _categoryRepositoryGenericMock
                .Verify(x => x.GetByIdAsync(categoryId), times: Times.AtLeastOnce);

            _categoryRepositoryMock
                .Verify(x => x.GetCategoryByName(categoryDto.Name), times: Times.AtLeastOnce);
        }
    }
}
