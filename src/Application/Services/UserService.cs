using SGS.TaskTracker.Common.Mappings;
using SGS.TaskTracker.Dtos;
using SGS.TaskTracker.Interfaces;

namespace SGS.TaskTracker.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            if (await _userRepository.UserExistsAsync(userCreateDto.Username, userCreateDto.Email))
            {
                throw new InvalidOperationException("User with this username or email already exists");
            }

            var user = userCreateDto.ToUser();
            user.PasswordHash = _passwordHasher.HashPassword(userCreateDto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.ToUserResponseDto();
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.ToUserResponseDto();
        }

        public async Task<UserResponseDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user?.ToUserResponseDto();
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.ToUserResponseDtos();
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.UpdateFromDto(userUpdateDto);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return user.ToUserResponseDto();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            _userRepository.Delete(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id) != null;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            _userRepository.Update(user);
            return await _userRepository.SaveChangesAsync() > 0;
        }
    }
}
