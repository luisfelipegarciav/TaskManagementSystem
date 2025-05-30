using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Domain;
using Xunit.Sdk;

namespace TaskManagementSystem.Application.UnitTests.Services
{
    public class UserServiceUnitTests
    {
        private readonly Mock<IRepository<User>> _mockUserRespositoryGeneric;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITaskItemService> _mockTaskItemService;
        private readonly UserService _userService;

        public UserServiceUnitTests()
        {
            _mockUserRespositoryGeneric = new Mock<IRepository<User>>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTaskItemService = new Mock<ITaskItemService>();
            _userService = new UserService(
                _mockLogger.Object,
                _mockUserRespositoryGeneric.Object,
                _mockPasswordHasher.Object,
                _mockUserRepository.Object,
                _mockTaskItemService.Object
                );
        }

        [Fact]
        public async Task Handle_UserService_ChangePasswordAsync_Failure_Invalidmodel()
        {
            // Arrange
            var expectedLogMessage = $"{nameof(UserService)}.ChangePasswordAsync";
            var expectedExceptionMessage = "Invalid model.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 0;

            // Act
            var result = await _userService.ChangePasswordAsync(userId, (ChangePasswordRequestDto?)null);

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
        public async Task Handle_UserService_ChangePasswordAsync_Failure_UserNotFound()
        {
            // Arrange
            var expectedLogMessage = $"{nameof(UserService)}.ChangePasswordAsync";
            var expectedExceptionMessage = "User not found.";
            var expectedException = new EntityNotFoundException(expectedExceptionMessage);

            var userId = 1;
            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            _mockUserRespositoryGeneric.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.ChangePasswordAsync(userId, request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockUserRespositoryGeneric
                .Verify(x => x.GetByIdAsync(userId), times: Times.AtLeastOnce);

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
        public async Task Handle_UserService_ChangePasswordAsync_Failure_UserNotFound_Hash()
        {
            // Arrange
            var expectedLogMessage = $"{nameof(UserService)}.ChangePasswordAsync";
            var expectedExceptionMessage = "User not found.";
            var expectedException = new EntityNotFoundException(expectedExceptionMessage);

            var userId = 1;
            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            var user = new User
            {
                Id = userId,
                Username = "testUser",
                PasswordHash = "hashedPassword"
            };

            _mockUserRespositoryGeneric.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword))
                .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = await _userService.ChangePasswordAsync(userId, request);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockUserRespositoryGeneric
                .Verify(x => x.GetByIdAsync(userId), times: Times.AtLeastOnce);

            _mockPasswordHasher
                .Verify(x => x.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword), times: Times.AtLeastOnce);

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
        public async Task Handle_UserService_ChangePasswordAsync_Success()
        {
            // Arrange
            var userId = 1;

            var request = new ChangePasswordRequestDto
            {
                CurrentPassword = "oldPassword",
                NewPassword = "newPassword"
            };

            var user = new User
            {
                Id = userId,
                Username = "testUser",
                PasswordHash = "hashedPassword"
            };

            _mockUserRespositoryGeneric.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            _mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(user, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);

            _mockPasswordHasher
                .Setup(x => x.HashPassword(user, It.IsAny<string>()))
                .Returns("abc");

            _mockUserRepository
                .Setup(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>()));

            // Act
            var result = await _userService.ChangePasswordAsync(userId, request);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _mockUserRespositoryGeneric
                .Verify(x => x.GetByIdAsync(userId), times: Times.AtLeastOnce);

            _mockPasswordHasher
                .Verify(x => x.VerifyHashedPassword(user, It.IsAny<string>(), It.IsAny<string>()), times: Times.AtLeastOnce);

            _mockPasswordHasher
                .Verify(x => x.HashPassword(user, It.IsAny<string>()), times: Times.AtLeastOnce);

            _mockUserRepository
                .Verify(x => x.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>()), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UserService_CreateUserAsync_Failure_InvalidData()
        {
            // Arrange
            var expectedLogMessage = "Error creating user.";
            var expectedExceptionMessage = "Invalid user data.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var data = SetupCreateUserDto("InvalidData");

            // Act
            var result = await _userService.CreateUserAsync(data);

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
        public async Task Handle_UserService_CreateUserAsync_Failure_InvalidUserName()
        {
            // Arrange
            var expectedLogMessage = "Error creating user.";
            var expectedExceptionMessage = "Invalid username";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var data = SetupCreateUserDto("InvalidUserName");

            // Act
            var result = await _userService.CreateUserAsync(data);

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
        public async Task Handle_UserService_CreateUserAsync_Failure_InvalidEmail()
        {
            // Arrange
            var expectedLogMessage = "Error creating user.";
            var expectedExceptionMessage = "Invalid email";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var data = SetupCreateUserDto("InvalidEmail");

            // Act
            var result = await _userService.CreateUserAsync(data);

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
        public async Task Handle_UserService_CreateUserAsync_Failure_InvalidPassword()
        {
            // Arrange
            var expectedLogMessage = "Error creating user.";
            var expectedExceptionMessage = "Invalid password";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var data = SetupCreateUserDto("InvalidPassword");

            // Act
            var result = await _userService.CreateUserAsync(data);

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
        public async Task Handle_UserService_CreateUserAsync_Failure_InvalidRoles()
        {
            // Arrange
            var expectedLogMessage = "Error creating user.";
            var expectedExceptionMessage = "Roles are required.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var data = SetupCreateUserDto("InvalidRoles");

            // Act
            var result = await _userService.CreateUserAsync(data);

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
        public async Task Handle_UserService_CreateUserAsync_Failure_RoleNotFound()
        {
            // Arrange
            var roleName = "abc";
            var expectedLogMessage = "Error creating user.";
            var expectedExceptionMessage = $"Role {roleName} not found.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var data = SetupCreateUserDto();

            _mockUserRepository
                .Setup(x => x.GetRoleByNameAsync(roleName))
                .ReturnsAsync((Role?)null);

            // Act
            var result = await _userService.CreateUserAsync(data);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockUserRepository
                .Verify(x => x.GetRoleByNameAsync(roleName), times: Times.AtLeastOnce);

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
        public async Task Handle_UserService_CreateUserAsync_Failure_UserNotCreated()
        {
            // Arrange
            var expectedLogMessage = "Error creating user.";
            var expectedExceptionMessage = "User not created.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var data = SetupCreateUserDto();

            _mockUserRepository
                .Setup(x => x.GetRoleByNameAsync(It.IsAny<string>()))
                .ReturnsAsync( new Role { Name = "abc", Id = 1 });

            _mockPasswordHasher
                .Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("abc");

            _mockUserRespositoryGeneric
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.CreateUserAsync(data);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockUserRepository
                .Verify(x => x.GetRoleByNameAsync(It.IsAny<string>()), times: Times.AtLeastOnce);

            _mockPasswordHasher
                .Verify(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>()), times: Times.AtLeastOnce);

            _mockUserRespositoryGeneric
                .Verify(x => x.AddAsync(It.IsAny<User>()), times: Times.AtLeastOnce);

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
        public async Task Handle_UserService_CreateUserAsync_Success()
        {
            // Arrange

            var data = SetupCreateUserDto();

            _mockUserRepository
                .Setup(x => x.GetRoleByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new Role { Name = "abc", Id = 1 });

            _mockPasswordHasher
                .Setup(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("abc");

            _mockUserRespositoryGeneric
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .ReturnsAsync(new User() { Id = 1, Email = "abc", Username = "qwerty" });

            _mockUserRepository
                .Setup(x => x.AddUserRoleAsync(It.IsAny<UserRole>()));

            // Act
            var result = await _userService.CreateUserAsync(data);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            _mockUserRepository
                .Verify(x => x.GetRoleByNameAsync(It.IsAny<string>()), times: Times.AtLeastOnce);

            _mockPasswordHasher
                .Verify(x => x.HashPassword(It.IsAny<User>(), It.IsAny<string>()), times: Times.AtLeastOnce);

            _mockUserRespositoryGeneric
                .Verify(x => x.AddAsync(It.IsAny<User>()), times: Times.AtLeastOnce);

            _mockUserRepository
                .Verify(x => x.AddUserRoleAsync(It.IsAny<UserRole>()), times: Times.AtLeastOnce);
        }

        private CreateUserDto SetupCreateUserDto(string testCase = "OK")
        {
            var data = new CreateUserDto
            {
                Email = "mymail@domain.com",
                Password = "password",
                Roles = new List<string>() { "abc" },
                Username = "abc"
            };

            switch (testCase)
            {
                case "InvalidData":
                    data = null;
                    break;
                case "InvalidUserName":
                    data.Username = string.Empty;
                    break;
                case "InvalidEmail":
                    data.Email = string.Empty;
                    break;
                case "InvalidPassword":
                    data.Password = string.Empty;
                    break;
                case "InvalidRoles":
                    data.Roles = new List<string>();
                    break;
                default:
                    break;
            }

            return data;
        }

        [Fact]
        public async Task Handle_UserService_DeleteUserByIdAsync_Failure_InvalidUser()
        {
            // Arrange
            var expectedLogMessage = $"{nameof(UserService)}.DeleteUserByIdAsync";
            var expectedExceptionMessage = "Invalid user id.";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userId = 0;

            // Act
            var result = await _userService.DeleteUserByIdAsync(userId);

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
        public async Task Handle_UserService_DeleteUserByIdAsync_Failure_UserHasTasks()
        {
            // Arrange
            var expectedExceptionMessage = "User cannot be deleted because it has task items.";

            var userId = 1;

            _mockTaskItemService
                .Setup(x => x.GetTaskItemsCountByUserIdAsync(userId))
                .ReturnsAsync(ServiceResponse<int>.Success(data: 1));


            // Act
            var result = await _userService.DeleteUserByIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.False(result.Data);
            Assert.Equal(expectedExceptionMessage, result.Message);

            _mockTaskItemService
                .Verify(x => x.GetTaskItemsCountByUserIdAsync(userId), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UserService_DeleteUserByIdAsync_Success()
        {
            // Arrange

            var userId = 1;

            _mockTaskItemService
                .Setup(x => x.GetTaskItemsCountByUserIdAsync(userId))
                .ReturnsAsync(ServiceResponse<int>.Failure());

            _mockUserRepository
                .Setup(x => x.DeleteUserRolesByIdAsync(userId));

            _mockUserRespositoryGeneric
                .Setup(x => x.DeleteAsync(userId));

            // Act
            var result = await _userService.DeleteUserByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _mockTaskItemService
                .Verify(x => x.GetTaskItemsCountByUserIdAsync(userId), times: Times.AtLeastOnce);

            _mockUserRepository
                .Verify(x => x.DeleteUserRolesByIdAsync(userId), times: Times.AtLeastOnce);

            _mockUserRespositoryGeneric
                .Verify(x => x.DeleteAsync(userId), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UserService_GetByIdAsync_Failure()
        {
            // Arrange
            var expectedLogMessage = $"{nameof(UserService)}.GetByUsernameAsync";
            var userId = 1;

            _mockUserRespositoryGeneric
                .Setup(x => x.GetByIdAsync(userId))
                .ThrowsAsync(It.IsAny<Exception>());


            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);

            _mockUserRespositoryGeneric
                .Verify(x => x.GetByIdAsync(userId), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.IsAny<Exception>(), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UserService_GetByIdAsync_Success()
        {
            // Arrange
            var userId = 1;

            _mockUserRespositoryGeneric
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });


            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Equal(userId, result.Data.Id);
            Assert.Null(result.Message);

            _mockUserRespositoryGeneric
                .Verify(x => x.GetByIdAsync(userId), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UserService_GetRolesByUserId_Failure()
        {
            // Arrange
            var expectedLogMessage = $"{nameof(UserService)}.GetByRolesByUserId";
            var userId = 1;

            _mockUserRepository
                .Setup(x => x.GetUserRolesAsync(userId))
                .ThrowsAsync(It.IsAny<Exception>());


            // Act
            var result = await _userService.GetByRolesByUserId(userId);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);

            _mockUserRepository
                .Verify(x => x.GetUserRolesAsync(userId), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.IsAny<Exception>(), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UserService_GetRolesByUserId_Success()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository
                .Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync(new List<Role> { new Role { Id = 1, Name = "Admin" } });


            // Act
            var result = await _userService.GetByRolesByUserId(userId);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Null(result.Message);

            _mockUserRepository
                .Verify(x => x.GetUserRolesAsync(userId), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UserService_GetByUsernameAsync_Failure()
        {
            // Arrange
            var expectedLogMessage = $"{nameof(UserService)}.GetByUsernameAsync";
            var userName = "qwerty";

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(userName))
                .ThrowsAsync(It.IsAny<Exception>());


            // Act
            var result = await _userService.GetByUsernameAsync(userName);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Null(result.Data);
            Assert.NotEmpty(result.Message);

            _mockUserRepository
                .Verify(x => x.GetByUsernameAsync(userName), times: Times.AtLeastOnce);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                    It.IsAny<Exception>(), // Verify the specific exception  
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_UserService_GetByUsernameAsync_Success()
        {
            // Arrange
            var userName = "qwerty";

            _mockUserRepository
                .Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync(new User { Id = 1, Username = userName });


            // Act
            var result = await _userService.GetByUsernameAsync(userName);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Equal(userName, result.Data.Username);
            Assert.Null(result.Message);

            _mockUserRepository
                .Verify(x => x.GetByUsernameAsync(userName), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UserService_GetUsersAsync_Failure()
        {
            // Arrange
            var expectedExceptionMessage = "Invalid pagination params.";
            var expectedLogMessage = $"{nameof(UserService)}.GetUsersAsync";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            // Act
            var result = await _userService.GetUsersAsync(null);

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
        public async Task Handle_UserService_GetUsersAsync_Success()
        {
            // Arrange
            var paginationParams = new PaginationParamsDto
            {
                PageNumber = 1,
                PageSize = 5
            };

            var rowsOffset = (paginationParams.PageNumber - 1) * paginationParams.PageSize;

            _mockUserRepository
                .Setup(x => x.GetUsersCountAsync())
                .ReturnsAsync(10);

            _mockUserRepository
                .Setup(x => x.GetUsersAsync(rowsOffset, paginationParams.PageSize))
                .ReturnsAsync(new List<User> { new User { Id = 1, Username = "qwerty", Email = "my@mail.com" } });

            // Act
            var result = await _userService.GetUsersAsync(paginationParams);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);

            _mockUserRepository
                .Verify(x => x.GetUsersCountAsync(), times: Times.AtLeastOnce);

            _mockUserRepository
                .Verify(x => x.GetUsersAsync(rowsOffset, paginationParams.PageSize), times: Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UserService_UpdateUserAsync_Failure_InvalidUserData()
        {
            // Arrange
            var expectedExceptionMessage = "Invalid user data.";
            var expectedLogMessage = "Error updating user";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            // Act
            var result = await _userService.UpdateUserAsync(null);

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
        public async Task Handle_UserService_UpdateUserAsync_Failure_InvalidEmail()
        {
            // Arrange
            var expectedExceptionMessage = "Invalid email";
            var expectedLogMessage = "Error updating user";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userToUpdate = new UpdateUserDto
            {
                Email = string.Empty,
                Id = 1,
            };

            // Act
            var result = await _userService.UpdateUserAsync(userToUpdate);

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
        public async Task Handle_UserService_UpdateUserAsync_Failure_InvalidRoles()
        {
            // Arrange
            var expectedExceptionMessage = "Roles are required.";
            var expectedLogMessage = "Error updating user";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userToUpdate = new UpdateUserDto
            {
                Email = "my@mail.com",
                Id = 1,
            };

            // Act
            var result = await _userService.UpdateUserAsync(userToUpdate);

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
        public async Task Handle_UserService_UpdateUserAsync_Failure_InvalidRolNotFound()
        {
            // Arrange
            var role = "abc";
            var expectedExceptionMessage = $"Role {role} not found.";
            var expectedLogMessage = "Error updating user";
            var expectedException = new InvalidModelException(expectedExceptionMessage);

            var userToUpdate = new UpdateUserDto
            {
                Email = "my@mail.com",
                Id = 1,
                Roles = new List<string> { role }
            };

            _mockUserRepository
                .Setup(x => x.GetRoleByNameAsync(role))
                .ReturnsAsync((Role?)null);

            // Act
            var result = await _userService.UpdateUserAsync(userToUpdate);

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
        public async Task Handle_UserService_UpdateUserAsync_Failure_UserNotFound()
        {
            // Arrange
            var role = "abc";   
            var expectedExceptionMessage = "User not found.";
            var expectedLogMessage = "Error updating user";
            var expectedException = new EntityNotFoundException(expectedExceptionMessage);
            var userId = 1;

            var userToUpdate = new UpdateUserDto
            {
                Email = "my@mail.com",
                Id = userId,
                Roles = new List<string> { role }
            };

            _mockUserRepository
                .Setup(x => x.GetRoleByNameAsync(role))
                .ReturnsAsync(new Role { Id = 1, Name = role });

            _mockUserRespositoryGeneric
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.UpdateUserAsync(userToUpdate);

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
        public async Task Handle_UserService_UpdateUserAsync_Success()
        {
            // Arrange
            var role = "abc";
            var userId = 1;

            var userToUpdate = new UpdateUserDto
            {
                Email = "my@mail.com",
                Id = userId,
                Roles = new List<string> { role }
            };

            _mockUserRepository
                .Setup(x => x.GetRoleByNameAsync(role))
                .ReturnsAsync(new Role { Id = 1, Name = role });

            _mockUserRespositoryGeneric
                .Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new User { Email = "email@domain.com", Id= userId });

            _mockUserRespositoryGeneric
                .Setup(x => x.UpdateAsync(It.IsAny<User>()));

            _mockUserRepository
                .Setup(x => x.DeleteUserRolesByIdAsync(userId));

            _mockUserRepository
                .Setup(x => x.AddUserRoleAsync(It.IsAny<UserRole>()));

            // Act
            var result = await _userService.UpdateUserAsync(userToUpdate);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.True(result.Data);
            Assert.Null(result.Message);

            _mockUserRepository
                .Verify(x => x.GetRoleByNameAsync(role), times: Times.AtLeastOnce);

            _mockUserRespositoryGeneric
                .Verify(x => x.GetByIdAsync(userId), times: Times.AtLeastOnce);

            _mockUserRespositoryGeneric
                .Verify(x => x.UpdateAsync(It.IsAny<User>()), times: Times.AtLeastOnce);

            _mockUserRepository
                .Verify(x => x.DeleteUserRolesByIdAsync(userId), times: Times.AtLeastOnce);

            _mockUserRepository
                .Verify(x => x.AddUserRoleAsync(It.IsAny<UserRole>()), times: Times.AtLeastOnce);
        }
    }
}
