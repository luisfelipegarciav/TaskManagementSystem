using Moq;

namespace TaskManagementSystem.Application.UnitTests
{
    public class AuthenticateUserCommandUnitTests
    {
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly AuthenticateUserCommandHandler _handler;

        public AuthenticateUserCommandUnitTests()
        {
            // Arrange: Create a mock of the IAuthenticationService
            _mockAuthService = new Mock<IAuthenticationService>();

            // Arrange: Create an instance of the command handler with the mocked service
            _handler = new AuthenticateUserCommandHandler(_mockAuthService.Object);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsSuccessTokenResponse()
        {
            // Arrange
            var command = new AuthenticateUserCommand("testuser", "testpassword");
            var expectedTokenResponse = new TokenResponse { AccessToken = "fake_access_token", RefreshToken = "fake_refresh_token" };
            var expectedServiceResponse = ServiceResponse<TokenResponse>.Success(expectedTokenResponse);

            // Set up the mock to return a successful response
            _mockAuthService.Setup(x => x.AuthenticateAsync(command.Username, command.Password))
                .ReturnsAsync(expectedServiceResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal(expectedTokenResponse.AccessToken, result.Data.AccessToken);
            Assert.Equal(expectedTokenResponse.RefreshToken, result.Data.RefreshToken);

            // Verify that the AuthenticateAsync method was called once with the correct arguments
            _mockAuthService.Verify(
                x => x.AuthenticateAsync(command.Username, command.Password),
                Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCredentials_ReturnsFailureServiceResponse()
        {
            // Arrange
            var command = new AuthenticateUserCommand("wronguser", "wrongpassword");
            var expectedErrorMessage = "Invalid credentials";
            var expectedServiceResponse = ServiceResponse<TokenResponse>.Failure(expectedErrorMessage);

            // Set up the mock to return a failure response
            _mockAuthService.Setup(x => x.AuthenticateAsync(command.Username, command.Password))
                .ReturnsAsync(expectedServiceResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(expectedErrorMessage, result.Message);
            Assert.Null(result.Data);

            // Verify that the AuthenticateAsync method was called once with the correct arguments
            _mockAuthService.Verify(
                x => x.AuthenticateAsync(command.Username, command.Password),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationServiceReturnsFailureOnError_ReturnsFailureServiceResponse()
        {
            // Arrange
            var command = new AuthenticateUserCommand("erroruser", "errorpassword");
            var expectedErrorMessage = "Authentication failed due to an internal error.";
            var expectedServiceResponse = ServiceResponse<TokenResponse>.Failure(expectedErrorMessage);

            // Set up the mock to return a failure response indicating an error
            _mockAuthService.Setup(x => x.AuthenticateAsync(command.Username, command.Password))
                .ReturnsAsync(expectedServiceResponse); // Simulate service returning failure

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(expectedErrorMessage, result.Message);
            Assert.Null(result.Data);

            // Verify that the AuthenticateAsync method was called once with the correct arguments
            _mockAuthService.Verify(
                x => x.AuthenticateAsync(command.Username, command.Password),
                Times.Once);
        }

    }
}
