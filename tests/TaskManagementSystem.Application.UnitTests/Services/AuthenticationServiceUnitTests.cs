using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application.UnitTests.Services
{
    public class AuthenticationServiceUnitTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly Mock<ILogger<AuthenticationService>> _mockLogger;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceUnitTests()
        {
            _mockJwtService = new Mock<IJwtService>();
            _mockLogger = new Mock<ILogger<AuthenticationService>>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _mockUserService = new Mock<IUserService>();

            _authenticationService = new AuthenticationService(
                _mockUserService.Object,
                _mockJwtService.Object,
                _mockPasswordHasher.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task Handle_AuthenticationService_AuthenticateAsync_LogException()
        {
            // Arrange
            (string userName, string password) = generateUserCredentials();

            var expectedException = new Exception("Some exception");
            var expecteLogMessage = $"Error authenticating user '{userName}'.";
            var expecteErrorMessage = "An unexpected error occurred during authentication.";
            var expectedServiceResponse = ServiceResponse<User>.Failure(expecteErrorMessage);

            _mockUserService
                .Setup(x => x.GetByUsernameAsync(userName))
                .Throws(expectedException);

            // Act
            var result = await _authenticationService.AuthenticateAsync(userName, password);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);
            _mockUserService
                .Verify(x => x.GetByUsernameAsync(userName), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.Is<Exception>(ex => ex == expectedException), // Verify the specific exception
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_AuthenticateAsync_Failure_GetUserByName()
        {
            // Arrange
            (string userName, string password) = generateUserCredentials();

            var expecteLogMessage = $"Authentication failed: User '{userName}' not found.";
            var expecteErrorMessage = "Invalid username or password.";
            var expectedServiceResponse = ServiceResponse<User>.Failure(expecteErrorMessage);

            _mockUserService
                .Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync(expectedServiceResponse);

            // Act
            var result = await _authenticationService.AuthenticateAsync(userName, password);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);
            _mockUserService
                .Verify(x => x.GetByUsernameAsync(userName), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning, // Specify LogLevel.Warning
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.IsAny<Exception>(), // LogWarning usually doesn't have an exception
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_AuthenticateAsync_Failure_InactiveUser()
        {
            // Arrange
            (string userName, string password) = generateUserCredentials();
            var user = getByUsernameAsyncResponse();
            user.Status = Status.Inactive;
            var expecteLogMessage = $"Authentication failed: User status is not active.";
            var expecteErrorMessage = "Invalid user status.";
            var expectedServiceResponse = ServiceResponse<User>.Success(user);

            _mockUserService
                .Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync(expectedServiceResponse);

            // Act
            var result = await _authenticationService.AuthenticateAsync(userName, password);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);
            _mockUserService
                .Verify(x => x.GetByUsernameAsync(userName), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning, // Specify LogLevel.Warning
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.IsAny<Exception>(), // LogWarning usually doesn't have an exception
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_AuthenticateAsync_Failure_BadHashedPassword()
        {
            // Arrange
            (string userName, string password) = generateUserCredentials();
            var user = getByUsernameAsyncResponse();
            var expecteLogMessage = $"Authentication failed: Invalid password for user '{userName}'.";
            var expecteErrorMessage = "Invalid username or password.";
            var expectedServiceResponse = ServiceResponse<User>.Success(user);

            _mockUserService
                .Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync(expectedServiceResponse);

            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, password))
                .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = await _authenticationService.AuthenticateAsync(userName, password);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expecteErrorMessage, result.Message);
            _mockUserService
                .Verify(x => x.GetByUsernameAsync(userName), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning, // Specify LogLevel.Warning
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expecteLogMessage)),
                    It.IsAny<Exception>(), // LogWarning usually doesn't have an exception
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_AuthenticateAsync_Success()
        {
            // Arrange
            (string userName, string password) = generateUserCredentials();
            var user = getByUsernameAsyncResponse();
            var expectedLogMessage = $"User '{userName}' authenticated successfully.";
            var expectedMessage = "Authentication successful.";
            var expectedServiceResponse = ServiceResponse<User>.Success(user);

            _mockUserService
                .Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync(expectedServiceResponse);

            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, password))
                .Returns(PasswordVerificationResult.Success);

            IEnumerable<Role> roles = new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                }
            };
            var getByRolesByUserIdResponse = ServiceResponse<IEnumerable<Role>>.Success();

            _mockUserService
                .Setup(x => x.GetByRolesByUserId(user.Id))
                .ReturnsAsync(getByRolesByUserIdResponse);

            var tokenResponse = new TokenResponse
            {
                AccessToken = "abc",
                Expiration = 999,
                RefreshToken = "qwerty"
            };

            _mockJwtService
                .Setup(x => x.GenerateJwtToken(user, It.IsAny<List<string>>()))
                .Returns(tokenResponse);

            // Act
            var result = await _authenticationService.AuthenticateAsync(userName, password);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Equal(expectedMessage, result.Message);
            _mockUserService
                .Verify(x => x.GetByUsernameAsync(userName), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information, // Specify LogLevel.Warning
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.IsAny<Exception>(), // LogWarning usually doesn't have an exception
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_RefreshTokenAsync_LogException()
        {
            // Arrange
            var request = generateRrefeshTokenRequest();
            var expectedLogMessage = $"Error on refresh token.";
            var expectedExceptionMessage = "Invalid Refresh Token.";
            var expectedException = new Exception(expectedExceptionMessage);

            _mockJwtService
                .Setup(x => x.GetPrincipalFromToken(request.Token))
                .Throws(expectedException);

            // Act
            var result = await _authenticationService.RefreshTokenAsync(request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            _mockJwtService
                .Verify(x => x.GetPrincipalFromToken(request.Token), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<Exception>(ex => ex == expectedException), // Verify the specific exception
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_RefreshTokenAsync_Failure_InvalidTokenClaims()
        {
            // Arrange
            var request = generateRrefeshTokenRequest();
            var expectedLogMessage = $"Error on refresh token.";
            var expectedExceptionMessage = "Invalid Refresh Token Claims.";
            var expectedException = new AuthenticationException(expectedExceptionMessage);
            var claimsPrincipal = generateClaimsPrincipal(false, false);

            _mockJwtService
                .Setup(x => x.GetPrincipalFromToken(request.Token))
                .Returns(claimsPrincipal);

            // Act
            var result = await _authenticationService.RefreshTokenAsync(request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            _mockJwtService
                .Verify(x => x.GetPrincipalFromToken(request.Token), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<AuthenticationException>(ex => ex.Message == expectedExceptionMessage), // Verify the specific exception
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_RefreshTokenAsync_Failure_InvalidExpirationClaims()
        {
            // Arrange
            var request = generateRrefeshTokenRequest();
            var expectedLogMessage = $"Error on refresh token.";
            var expectedExceptionMessage = "Refresh token has no expiry claim.";
            var expectedException = new AuthenticationException(expectedExceptionMessage);
            var claimsPrincipal = generateClaimsPrincipal(addExpiration: false);

            _mockJwtService
                .Setup(x => x.GetPrincipalFromToken(request.Token))
                .Returns(claimsPrincipal);

            // Act
            var result = await _authenticationService.RefreshTokenAsync(request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            _mockJwtService
                .Verify(x => x.GetPrincipalFromToken(request.Token), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<AuthenticationException>(ex => ex.Message == expectedExceptionMessage), // Verify the specific exception
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_RefreshTokenAsync_Failure_ExpirationClaimExpired()
        {
            // Arrange
            var request = generateRrefeshTokenRequest();
            var expectedLogMessage = $"Error on refresh token.";
            var expectedExceptionMessage = "Refresh token has expired.";
            var expectedException = new AuthenticationException(expectedExceptionMessage);
            var claimsPrincipal = generateClaimsPrincipal(addExpiredJwt: true);

            _mockJwtService
                .Setup(x => x.GetPrincipalFromToken(request.Token))
                .Returns(claimsPrincipal);

            // Act
            var result = await _authenticationService.RefreshTokenAsync(request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            _mockJwtService
                .Verify(x => x.GetPrincipalFromToken(request.Token), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<AuthenticationException>(ex => ex.Message == expectedExceptionMessage), // Verify the specific exception
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_RefreshTokenAsync_Failure_UserNotAssociatedToken()
        {
            // Arrange
            var request = generateRrefeshTokenRequest();
            var expectedLogMessage = $"Error on refresh token.";
            var expectedExceptionMessage = "User associated with refresh token not found.";
            var expectedException = new AuthenticationException(expectedExceptionMessage);
            var claimsPrincipal = generateClaimsPrincipal();

            _mockJwtService
                .Setup(x => x.GetPrincipalFromToken(request.Token))
                .Returns(claimsPrincipal);

            var getUserByIdResponse = ServiceResponse<User>.Failure();
            _mockUserService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(getUserByIdResponse);

            // Act
            var result = await _authenticationService.RefreshTokenAsync(request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            _mockJwtService
                .Verify(x => x.GetPrincipalFromToken(request.Token), Times.Once);
            _mockUserService
                .Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.Is<AuthenticationException>(ex => ex.Message == expectedExceptionMessage), // Verify the specific exception
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_AuthenticationService_RefreshTokenAsync_Success()
        {
            // Arrange
            var request = generateRrefeshTokenRequest();
            var claimsPrincipal = generateClaimsPrincipal();

            _mockJwtService
                .Setup(x => x.GetPrincipalFromToken(request.Token))
                .Returns(claimsPrincipal);

            var getUserByIdResponse = ServiceResponse<User>.Success(new User
            {
                Email = "testUser",
                Id = 1,
                Status = Status.Active,
                Username = "testUser",
            });

            _mockUserService
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(getUserByIdResponse);

            var getByRolesByUserIdResponse = ServiceResponse<IEnumerable<Role>>.Success(new List<Role>
            {
                new Role
                {
                    Id = 1,
                    Name = "Admin",
                }
            });

            _mockUserService
                .Setup(x => x.GetByRolesByUserId(It.IsAny<int>()))
                .ReturnsAsync(getByRolesByUserIdResponse);

            var tokenResponse = new TokenResponse
            {
                AccessToken = "abc",
                Expiration = 999,
                RefreshToken = "qwerty"
            };

            _mockJwtService
                .Setup(x => x.GenerateJwtToken(It.IsAny<User>(), It.IsAny<List<string>>()))
                .Returns(tokenResponse);

            // Act
            var result = await _authenticationService.RefreshTokenAsync(request);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            _mockJwtService
                .Verify(x => x.GetPrincipalFromToken(request.Token), Times.Once);
            _mockUserService
                .Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _mockUserService
                .Verify(x => x.GetByRolesByUserId(It.IsAny<int>()), Times.Once);
            _mockJwtService
                .Verify(x => x.GenerateJwtToken(It.IsAny<User>(), It.IsAny<List<string>>()), Times.Once);
        }

        private (string, string) generateUserCredentials()
        {
            var username = "testUser";
            var password = "testPassword";
            return (username, password);
        }

        private User getByUsernameAsyncResponse()
        {
            return new User
            {
                Id = 1,
                Username = "testUser",
                PasswordHash = "hashedPassword",
                Status = Status.Active,
            };
        }

        private RefreshTokenRequestDto generateRrefeshTokenRequest()
        {
            return new RefreshTokenRequestDto
            { Token = "abc" };
        }

        private ClaimsPrincipal generateClaimsPrincipal(
            bool addNameIdentifier = true,
            bool addExpiration = true,
            bool addExpiredJwt = false
            )
        {
            var claims = new List<Claim>();
            if (addNameIdentifier)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, "1"));
            }
            if (addExpiration)
            {
                // Calculate the expiration time in UTC
                DateTimeOffset expirationTime = DateTime.UtcNow.AddDays(1);
                if (addExpiredJwt)
                {
                    expirationTime = DateTime.UtcNow.AddDays(-2);
                }

                // Convert the expiration time to Unix timestamp (seconds since epoch)
                long expiryUnix = expirationTime.ToUnixTimeSeconds();

                // Add the expiration claim using JwtRegisteredClaimNames.Exp
                claims.Add(new Claim(JwtRegisteredClaimNames.Exp, expiryUnix.ToString()));
            }
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
    }
}
