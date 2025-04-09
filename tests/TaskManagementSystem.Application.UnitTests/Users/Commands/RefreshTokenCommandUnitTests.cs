using Moq;

namespace TaskManagementSystem.Application.UnitTests
{
    public class RefreshTokenCommandUnitTests
    {
        private readonly Mock<IAuthenticationService> _mockAuthentication;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandUnitTests()
        {
            _mockAuthentication = new Mock<IAuthenticationService>();

            _handler = new RefreshTokenCommandHandler(_mockAuthentication.Object);
        }

        [Fact]
        public async Task Handle_RefreshToken_ReturnsSuccessResponse()
        {
            // Arrange
            var command = new RefreshTokenCommand(new RefreshTokenRequestDto
            {
                Token = "abc"
            });

            var expectedResult = new TokenResponse
            {
                AccessToken = "qwerty",
                Expiration = 999,
                RefreshToken = "xyz"
            };
            var expectdResponse = ServiceResponse<TokenResponse>.Success(expectedResult);

            _mockAuthentication
                .Setup(x => x.RefreshTokenAsync(command.dto))
                .ReturnsAsync(expectdResponse);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            _mockAuthentication.Verify(
                x => x.RefreshTokenAsync(command.dto),
                Times.Once);
        }
    }
}
