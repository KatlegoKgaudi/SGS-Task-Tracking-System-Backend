using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Entities;
using SGS.TaskTracker.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
