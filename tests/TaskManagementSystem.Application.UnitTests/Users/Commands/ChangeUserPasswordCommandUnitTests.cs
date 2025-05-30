using Moq;

namespace TaskManagementSystem.Application.UnitTests.Commands.Users
{
    public class ChangeUserPasswordCommandUnitTests
    {
        private readonly Mock<IUserService> _mockAuthService;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangeUserPasswordCommandUnitTests()
        {
            // Arrange: Create a mock of the service
            _mockAuthService = new Mock<IUserService>();

            // Arrange: Create an instance of the command handler with the mocked service
            _handler = new ChangePasswordCommandHandler(_mockAuthService.Object);
        }

        [Fact]
        public async Task Handle_ChangePassword_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new ChangeUserPasswordCommand(1, generateRequest());
            var expectedServiceResponse = ServiceResponse<bool>.Success(true);
            // Set up the mock to return a successful response
            _mockAuthService.Setup(x => x.ChangePasswordAsync(command.userId, command.dto))
                .ReturnsAsync(expectedServiceResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.True(string.IsNullOrWhiteSpace(result.Message));
            // Verify that the AuthenticateAsync method was called once with the correct arguments
            _mockAuthService.Verify(
                x => x.ChangePasswordAsync(command.userId, command.dto),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ChangePassword_ReturnsFailureResponse()
        {
            // Arrange
            var command = new ChangeUserPasswordCommand(1, generateRequest());
            var expectedMessage = "An error happened";
            var expectedServiceResponse = ServiceResponse<bool>.Failure(expectedMessage);
            // Set up the mock to return a successful response
            _mockAuthService.Setup(x => x.ChangePasswordAsync(command.userId, command.dto))
                .ReturnsAsync(expectedServiceResponse);
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.True(result.Message.Equals(expectedMessage, StringComparison.InvariantCulture));
            // Verify that the AuthenticateAsync method was called once with the correct arguments
            _mockAuthService.Verify(
                x => x.ChangePasswordAsync(command.userId, command.dto),
                Times.Once);
        }

        private ChangePasswordRequestDto generateRequest(string oldPassword = "currentPassword", string newPassword = "newPassword")
        {
            return new ChangePasswordRequestDto
            {
                CurrentPassword = oldPassword,
                NewPassword = newPassword
            };
        }
    }
}
