using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application.UnitTests.Services
{
    public class JwtServiceUnitService
    {
        private readonly Mock<ILogger<JwtService>> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly JwtService _jwtService;

        public JwtServiceUnitService()
        {
            _mockUserService = new Mock<ILogger<JwtService>>();
            _mockConfiguration = new Mock<IConfiguration>();

            var jwtSettings = new
            {
                SecretKey = "MMYz5IaDDucjbw05dT8CNvKBR4nJWfFIw9afBxC6pmo=",
                Issuer = "your-test-issuer",
                Audience = "your-test-audience",
            };

            _mockConfiguration.Setup(x => x["Jwt:SecretKey"]).Returns(jwtSettings.SecretKey);
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns(jwtSettings.Issuer);
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns(jwtSettings.Audience);

            _jwtService = new JwtService(_mockConfiguration.Object, _mockUserService.Object);
        }

        [Fact]
        public void Handle_JwtService_GenerateJwtToken_Success()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser"
            };
            var roles = new List<string> { "Admin", "User" };

            // Act
            var tokenResponse = _jwtService.GenerateJwtToken(user, roles);

            // Assert
            Assert.NotNull(tokenResponse);
            Assert.NotEmpty(tokenResponse.RefreshToken);
            Assert.NotEmpty(tokenResponse.AccessToken);
        }

    }
}
