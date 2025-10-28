using System.Security.Claims;
using Moq;
using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Core.Entities;
using SGS.TaskTracker.Application.Services;
using SGS.TaskTracker.Interfaces;
using Xunit;

namespace SGS.TaskTracker.Tests.UnitTests.Application.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();

            _authService = new AuthService(
                _mockUserRepository.Object,
                _mockRefreshTokenRepository.Object,
                _mockTokenService.Object,
                _mockPasswordHasher.Object);
        }

        public class RegisterAsync : AuthServiceTests
        {
            [Fact]
            public async Task RegisterAsync_ValidRequest_ReturnsSuccessResponse()
            {
                // Arrange
                var request = new UserRegisterRequest
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    Password = "password123",
                    ConfirmPassword = "password123"
                };

                _mockUserRepository.Setup(x => x.UserExistsAsync(request.Username, request.Email))
                    .ReturnsAsync(false);
                _mockPasswordHasher.Setup(x => x.HashPassword(request.Password))
                    .Returns("hashed_password");
                _mockTokenService.Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
                    .Returns("jwt_token");
                _mockTokenService.Setup(x => x.GenerateRefreshToken())
                    .Returns("refresh_token");

                // Act
                var result = await _authService.RegisterAsync(request);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Registration successful", result.Message);
                Assert.Equal("jwt_token", result.Token);
                Assert.Equal("refresh_token", result.RefreshToken);

                _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
                _mockUserRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
                _mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
                _mockRefreshTokenRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            }

            [Theory]
            [InlineData("", "test@example.com", "password123", "ConfirmPassword")]
            [InlineData("testuser", "", "password123", "password123")]
            [InlineData("testuser", "test@example.com", "", "")]
            [InlineData(null, "test@example.com", "password123", "password123")]
            public async Task RegisterAsync_MissingRequiredFields_ReturnsValidationError(
                string username, string email, string password, string confirmPassword)
            {
                // Arrange
                var request = new UserRegisterRequest
                {
                    Username = username,
                    Email = email,
                    Password = password,
                    ConfirmPassword = confirmPassword
                };

                // Act
                var result = await _authService.RegisterAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("All fields are required", result.Message);
            }

            [Fact]
            public async Task RegisterAsync_PasswordsDoNotMatch_ReturnsValidationError()
            {
                // Arrange
                var request = new UserRegisterRequest
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    Password = "password123",
                    ConfirmPassword = "differentpassword"
                };

                // Act
                var result = await _authService.RegisterAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Passwords do not match", result.Message);
            }

            [Fact]
            public async Task RegisterAsync_PasswordTooShort_ReturnsValidationError()
            {
                // Arrange
                var request = new UserRegisterRequest
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    Password = "123",
                    ConfirmPassword = "123"
                };

                // Act
                var result = await _authService.RegisterAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Password must be at least 6 characters long", result.Message);
            }

            [Fact]
            public async Task RegisterAsync_UserAlreadyExists_ReturnsValidationError()
            {
                // Arrange
                var request = new UserRegisterRequest
                {
                    Username = "existinguser",
                    Email = "existing@example.com",
                    Password = "password123",
                    ConfirmPassword = "password123"
                };

                _mockUserRepository.Setup(x => x.UserExistsAsync(request.Username, request.Email))
                    .ReturnsAsync(true);

                // Act
                var result = await _authService.RegisterAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Username or email already exists", result.Message);
            }
        }

        public class LoginAsync : AuthServiceTests
        {
            [Fact]
            public async Task LoginAsync_ValidCredentials_ReturnsSuccessResponse()
            {
                // Arrange
                var request = new UserLoginRequest
                {
                    Username = "testuser",
                    Password = "password123"
                };

                var user = new User { Id = 1, Username = "testuser", PasswordHash = "hashed_password" };

                _mockUserRepository.Setup(x => x.GetByUsernameAsync(request.Username))
                    .ReturnsAsync(user);
                _mockPasswordHasher.Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
                    .Returns(true);
                _mockTokenService.Setup(x => x.GenerateJwtToken(user))
                    .Returns("jwt_token");
                _mockTokenService.Setup(x => x.GenerateRefreshToken())
                    .Returns("refresh_token");

                // Act
                var result = await _authService.LoginAsync(request);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Login successful", result.Message);
                Assert.Equal("jwt_token", result.Token);
                Assert.Equal("refresh_token", result.RefreshToken);

                _mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
                _mockRefreshTokenRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task LoginAsync_InvalidUsername_ReturnsError()
            {
                // Arrange
                var request = new UserLoginRequest
                {
                    Username = "nonexistent",
                    Password = "password123"
                };

                _mockUserRepository.Setup(x => x.GetByUsernameAsync(request.Username))
                    .ReturnsAsync((User)null);

                // Act
                var result = await _authService.LoginAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Invalid username or password", result.Message);
            }

            [Fact]
            public async Task LoginAsync_InvalidPassword_ReturnsError()
            {
                // Arrange
                var request = new UserLoginRequest
                {
                    Username = "testuser",
                    Password = "wrongpassword"
                };

                var user = new User { Id = 1, Username = "testuser", PasswordHash = "hashed_password" };

                _mockUserRepository.Setup(x => x.GetByUsernameAsync(request.Username))
                    .ReturnsAsync(user);
                _mockPasswordHasher.Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
                    .Returns(false);

                // Act
                var result = await _authService.LoginAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Invalid username or password", result.Message);
            }
        }

        public class RefreshTokenAsync : AuthServiceTests
        {
            [Fact]
            public async Task RefreshTokenAsync_ValidTokens_ReturnsNewTokens()
            {
                // Arrange
                var request = new TokenRefreshRequest
                {
                    Token = "expired_token",
                    RefreshToken = "valid_refresh_token"
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }));

                var user = new User { Id = 1, Username = "testuser" };
                var storedRefreshToken = new RefreshToken
                {
                    Token = request.RefreshToken,
                    UserId = 1,
                    Expires = DateTime.UtcNow.AddDays(1),
                    IsRevoked = false
                };

                _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(request.Token))
                    .Returns(principal);
                _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(request.RefreshToken))
                    .ReturnsAsync(storedRefreshToken);
                _mockUserRepository.Setup(x => x.GetByIdAsync(1))
                    .ReturnsAsync(user);
                _mockTokenService.Setup(x => x.GenerateJwtToken(user))
                    .Returns("new_jwt_token");
                _mockTokenService.Setup(x => x.GenerateRefreshToken())
                    .Returns("new_refresh_token");

                // Act
                var result = await _authService.RefreshTokenAsync(request);

                // Assert
                Assert.True(result.Success);
                Assert.Equal("Token refreshed successfully", result.Message);
                Assert.Equal("new_jwt_token", result.Token);
                Assert.Equal("new_refresh_token", result.RefreshToken);

                _mockRefreshTokenRepository.Verify(x => x.Update(It.IsAny<RefreshToken>()), Times.Once);
                _mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
                _mockRefreshTokenRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task RefreshTokenAsync_InvalidToken_ReturnsError()
            {
                // Arrange
                var request = new TokenRefreshRequest
                {
                    Token = "invalid_token",
                    RefreshToken = "refresh_token"
                };

                _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(request.Token))
                    .Returns((ClaimsPrincipal)null);

                // Act
                var result = await _authService.RefreshTokenAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Invalid token", result.Message);
            }

            [Fact]
            public async Task RefreshTokenAsync_ExpiredRefreshToken_ReturnsError()
            {
                // Arrange
                var request = new TokenRefreshRequest
                {
                    Token = "expired_token",
                    RefreshToken = "expired_refresh_token"
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }));

                var storedRefreshToken = new RefreshToken
                {
                    Token = request.RefreshToken,
                    UserId = 1,
                    Expires = DateTime.UtcNow.AddDays(-1), // Expired
                    IsRevoked = false
                };

                _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(request.Token))
                    .Returns(principal);
                _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(request.RefreshToken))
                    .ReturnsAsync(storedRefreshToken);

                // Act
                var result = await _authService.RefreshTokenAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Invalid refresh token", result.Message);
            }

            [Fact]
            public async Task RefreshTokenAsync_RevokedRefreshToken_ReturnsError()
            {
                // Arrange
                var request = new TokenRefreshRequest
                {
                    Token = "expired_token",
                    RefreshToken = "revoked_refresh_token"
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }));

                var storedRefreshToken = new RefreshToken
                {
                    Token = request.RefreshToken,
                    UserId = 1,
                    Expires = DateTime.UtcNow.AddDays(1),
                    IsRevoked = true // Revoked
                };

                _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(request.Token))
                    .Returns(principal);
                _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(request.RefreshToken))
                    .ReturnsAsync(storedRefreshToken);

                // Act
                var result = await _authService.RefreshTokenAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Invalid refresh token", result.Message);
            }

            [Fact]
            public async Task RefreshTokenAsync_UserNotFound_ReturnsError()
            {
                // Arrange
                var request = new TokenRefreshRequest
                {
                    Token = "expired_token",
                    RefreshToken = "valid_refresh_token"
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "999") // Non-existent user
                }));

                var storedRefreshToken = new RefreshToken
                {
                    Token = request.RefreshToken,
                    UserId = 999,
                    Expires = DateTime.UtcNow.AddDays(1),
                    IsRevoked = false
                };

                _mockTokenService.Setup(x => x.GetPrincipalFromExpiredToken(request.Token))
                    .Returns(principal);
                _mockRefreshTokenRepository.Setup(x => x.GetByTokenAsync(request.RefreshToken))
                    .ReturnsAsync(storedRefreshToken);
                _mockUserRepository.Setup(x => x.GetByIdAsync(999))
                    .ReturnsAsync((User)null);

                // Act
                var result = await _authService.RefreshTokenAsync(request);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("User not found", result.Message);
            }
        }

        public class RevokeTokenAsync : AuthServiceTests
        {
            [Fact]
            public async Task RevokeTokenAsync_ValidToken_ReturnsTrue()
            {
                // Arrange
                var refreshToken = "valid_token";
                _mockRefreshTokenRepository.Setup(x => x.SaveChangesAsync())
                    .ReturnsAsync(1);

                // Act
                var result = await _authService.RevokeTokenAsync(refreshToken);

                // Assert
                Assert.True(result);
                _mockRefreshTokenRepository.Verify(x => x.RevokeTokenAsync(refreshToken), Times.Once);
            }

            [Fact]
            public async Task RevokeTokenAsync_NoChanges_ReturnsFalse()
            {
                // Arrange
                var refreshToken = "invalid_token";
                _mockRefreshTokenRepository.Setup(x => x.SaveChangesAsync())
                    .ReturnsAsync(0);

                // Act
                var result = await _authService.RevokeTokenAsync(refreshToken);

                // Assert
                Assert.False(result);
            }
        }

        public class LogoutAsync : AuthServiceTests
        {
            [Fact]
            public async Task LogoutAsync_ValidUserId_ReturnsTrue()
            {
                // Arrange
                var userId = 1;
                _mockRefreshTokenRepository.Setup(x => x.SaveChangesAsync())
                    .ReturnsAsync(1);

                // Act
                var result = await _authService.LogoutAsync(userId);

                // Assert
                Assert.True(result);
                _mockRefreshTokenRepository.Verify(x => x.RevokeAllTokensForUserAsync(userId), Times.Once);
            }

            [Fact]
            public async Task LogoutAsync_NoChanges_ReturnsFalse()
            {
                // Arrange
                var userId = 999;
                _mockRefreshTokenRepository.Setup(x => x.SaveChangesAsync())
                    .ReturnsAsync(0);

                // Act
                var result = await _authService.LogoutAsync(userId);

                // Assert
                Assert.False(result);
            }
        }
    }
}