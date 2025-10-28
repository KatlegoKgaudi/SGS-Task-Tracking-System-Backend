using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Core.Entities;
using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Core.Interfaces
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetTasksWithFiltersAsync(TaskFilterDto filter);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId);
        Task<bool> ExistsAsync(int id);
        Task<int> GetTaskCountByStatusAsync(TaskItemStatus status);
    }
}
