using Moq;

namespace TaskManagementSystem.Application.UnitTests.Commands.Users
{
    public class CreateUserCommandUnitTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandUnitTests()
        {
            _mockUserService = new Mock<IUserService>();

            _handler = new CreateUserCommandHandler(_mockUserService.Object);
        }

        [Fact]
        public async Task Handle_CreateUser_ReturnsSuccessResponse()
        {
            // Arrange
            var expectedUserId = 1;
            var command = new CreateUserCommand(generateRequest());
            var expectedServiceResponse = ServiceResponse<UserDto>.Success(generateResponse());
            _mockUserService.Setup(x => x.CreateUserAsync(command.dto))
                .ReturnsAsync(expectedServiceResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.True(result.Data.Id == expectedUserId);
            Assert.True(string.IsNullOrWhiteSpace(result.Message));
            _mockUserService.Verify(
                x => x.CreateUserAsync(command.dto),
                Times.Once);
        }

        [Fact]
        public async Task Handle_CreateUser_ReturnsFailureResponse()
        {
            // Arrange
            var expectedDefaultError = "An unexpected error occurred.";
            var command = new CreateUserCommand(generateRequest());
            var expectedServiceResponse = ServiceResponse<UserDto>.Failure();
            _mockUserService.Setup(x => x.CreateUserAsync(command.dto))
                .ReturnsAsync(expectedServiceResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
            Assert.Equal(result.Message, expectedDefaultError);
            _mockUserService.Verify(
                x => x.CreateUserAsync(command.dto),
                Times.Once);
        }

        private CreateUserDto generateRequest()
        {
            return new CreateUserDto
            {
                Email = "fake@email",
                Password = "fakePassword",
                Roles = new List<string> { "Admin" },
                Username = "fakeUser",
            };
        }

        private UserDto generateResponse()
        {
            return new UserDto
            {
                Id = 1,
                Email = "fake@email",
                Username = "fakeUser",
            };
        }
    }
}
