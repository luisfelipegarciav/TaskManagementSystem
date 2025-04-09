using Moq;

namespace TaskManagementSystem.Application.UnitTests
{
    public class GetUsersCommandUnitTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly GetUsersCommandHandler _handler;

        public GetUsersCommandUnitTests()
        {
            _mockUserService = new Mock<IUserService>();

            _handler = new GetUsersCommandHandler(_mockUserService.Object);
        }

        [Fact]
        public async Task Handle_GetUsers_ReturnsSuccessResponse()
        {
            // Arrange
            var expectedItemsResult = new PaginatedResultDto<UserDto>();
            expectedItemsResult.Items = new List<UserDto>()
            {
                new UserDto
                {
                    Email = "abc@mail.com",
                    Id = 1,
                    Username = "abc"
                }
            };
            expectedItemsResult.TotalCount = 1;

            var expectedItemsCount = expectedItemsResult.Items.Count;
            var pagination = new PaginationParamsDto();
            var command = new GetUsersCommand(pagination);
            var expectedResponse = ServiceResponse<PaginatedResultDto<UserDto>>.Success(expectedItemsResult);
            _mockUserService.Setup(x => x.GetUsersAsync(pagination))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Equal(result.Data.Items.Count, expectedItemsCount);
            Assert.Equal(result.Data.TotalCount, expectedItemsResult.TotalCount);
            Assert.True(string.IsNullOrWhiteSpace(result.Message));
            _mockUserService.Verify(
                x => x.GetUsersAsync(pagination),
                Times.Once);
        }

        [Fact]
        public async Task Handle_GetUsers_ReturnsFailureResponse()
        {
            // Arrange
            var expectedErrorMessage = "Unable to pull user records";
            var pagination = new PaginationParamsDto();
            var command = new GetUsersCommand(pagination);
            var expectedResponse = ServiceResponse<PaginatedResultDto<UserDto>>.Failure(expectedErrorMessage);
            _mockUserService.Setup(x => x.GetUsersAsync(pagination))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedErrorMessage, result.Message);
            _mockUserService.Verify(
                x => x.GetUsersAsync(pagination),
                Times.Once);
        }
    }
}
