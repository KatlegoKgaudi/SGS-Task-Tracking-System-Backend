using SGS.TaskTracker.Dtos;

namespace SGS.TaskTracker.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}