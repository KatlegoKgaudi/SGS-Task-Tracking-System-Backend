using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(int userId);
        Task<IEnumerable<TaskResponseDto>> GetTasksWithFiltersAsync(TaskFilterDto filter);
        Task<TaskResponseDto?> GetTaskByIdAsync(int id, int userId);
        Task<TaskResponseDto> CreateTaskAsync(TaskCreateDto taskCreateDto);
        Task<TaskResponseDto?> UpdateTaskAsync(int id, TaskUpdateDto taskUpdateDto, int userId);
        Task<bool> DeleteTaskAsync(int id, int userId);
        Task<TaskSummaryDto> GetTaskSummaryAsync(int userId);
        Task<bool> AssignTaskAsync(int taskId, int assignToUserId, int currentUserId);
        Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus status, int userId);

        // New methods for user-specific queries
        Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(int userId);
        Task<IEnumerable<TaskResponseDto>> GetTasksAssignedToUserAsync(int userId);
    }
}