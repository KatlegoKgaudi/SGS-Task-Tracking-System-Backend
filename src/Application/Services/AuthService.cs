using System.Security.Claims;
using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Core.Entities;
using SGS.TaskTracker.Dtos;
using SGS.TaskTracker.Interfaces;
using SGS.TaskTracker.Models;

namespace SGS.TaskTracker.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse> RegisterAsync(UserRegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new AuthResponse { Success = false, Message = "All fields are required" };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new AuthResponse { Success = false, Message = "Passwords do not match" };
            }

            if (request.Password.Length < 6)
            {
                return new AuthResponse { Success = false, Message = "Password must be at least 6 characters long" };
            }

            var existingUser = await _userRepository.UserExistsAsync(request.Username, request.Email);
            if (existingUser)
            {
                return new AuthResponse { Success = false, Message = "Username or email already exists" };
            }

            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                Role = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }


        public async Task<AuthResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return new AuthResponse { Success = false, Message = "Invalid username or password" };
            }

            var token = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(TokenRefreshRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
            {
                return new AuthResponse { Success = false, Message = "Invalid token" };
            }

            var userId = int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedRefreshToken == null || storedRefreshToken.UserId != userId || storedRefreshToken.IsRevoked || storedRefreshToken.Expires <= DateTime.UtcNow)
            {
                return new AuthResponse { Success = false, Message = "Invalid refresh token" };
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponse { Success = false, Message = "User not found" };
            }

            var newToken = _tokenService.GenerateJwtToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            storedRefreshToken.IsRevoked = true;
            _refreshTokenRepository.Update(storedRefreshToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
            return await _refreshTokenRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            await _refreshTokenRepository.RevokeAllTokensForUserAsync(userId);
            return await _refreshTokenRepository.SaveChangesAsync() > 0;
        }
    }
}