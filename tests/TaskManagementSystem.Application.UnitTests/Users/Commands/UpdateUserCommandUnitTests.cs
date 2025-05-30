using Moq;

namespace TaskManagementSystem.Application.UnitTests.Commands.Users
{
    public class UpdateUserCommandUnitTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandUnitTests()
        {
            _mockUserService = new Mock<IUserService>();

            _handler = new UpdateUserCommandHandler(_mockUserService.Object);
        }

        [Fact]
        public async Task Handle_UpdateUser_ReturnsSuccessResponse()
        {
            // Arrange
            var updateUserDtoRequest = generateRequest();
            var command = new UpdateUserCommand(updateUserDtoRequest);
            var expectedResponse = ServiceResponse<bool>.Success(true);
            _mockUserService.Setup(x => x.UpdateUserAsync(updateUserDtoRequest))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.True(string.IsNullOrWhiteSpace(result.Message));
            _mockUserService.Verify(
                x => x.UpdateUserAsync(updateUserDtoRequest),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateUser_ReturnsFailureResponse()
        {
            // Arrange
            var updateUserDtoRequest = generateRequest();
            var command = new UpdateUserCommand(updateUserDtoRequest);
            var expectedResponse = ServiceResponse<bool>.Failure();
            _mockUserService.Setup(x => x.UpdateUserAsync(updateUserDtoRequest))
                .ReturnsAsync(expectedResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
            _mockUserService.Verify(
                x => x.UpdateUserAsync(updateUserDtoRequest),
                Times.Once);
        }

        private UpdateUserDto generateRequest()
        {
            return new UpdateUserDto
            {
                Email = "fake@email",
                Roles = new List<string> { "Admin" }
            };
        }
    }
}
