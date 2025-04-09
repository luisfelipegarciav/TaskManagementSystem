using Moq;

namespace TaskManagementSystem.Application.UnitTests
{
    public class DeleteUserCommandUnitTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandUnitTests()
        {
            _mockUserService = new Mock<IUserService>();

            _handler = new DeleteUserCommandHandler(_mockUserService.Object);
        }

        [Fact]
        public async Task Handle_DeleteUser_ReturnsSuccessResponse()
        {
            // Arrange
            var id = 1;
            var command = new DeleteUserCommand(id);
            var expectedResponse = ServiceResponse<bool>.Success(true);

            _mockUserService.Setup(x => x.DeleteUserByIdAsync(id))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.True(string.IsNullOrWhiteSpace(result.Message));
            _mockUserService.Verify(
                x => x.DeleteUserByIdAsync(id),
                Times.Once);
        }

        [Fact]
        public async Task Handle_DeleteUser_ReturnsFailureResponse()
        {
            // Arrange
            var id = 1;
            var expectedMessage = "Can not delete this user";
            var command = new DeleteUserCommand(id);
            var expectedResponse = ServiceResponse<bool>.Failure(expectedMessage);

            _mockUserService.Setup(x => x.DeleteUserByIdAsync(id))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedMessage, result.Message);
            _mockUserService.Verify(
                x => x.DeleteUserByIdAsync(id),
                Times.Once);
        }
    }
}
