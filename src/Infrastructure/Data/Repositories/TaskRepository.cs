using Microsoft.EntityFrameworkCore;
using SGS.TaskTracker.Entities;
using SGS.TaskTracker.Enums;


namespace SGS.TaskTracker.Infrastructure.Data.Repositories
{
    public class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(TaskTrackerContext context) : base(context) { }

        public async Task<IEnumerable<TaskItem>> GetTasksWithFiltersAsync(TaskFilterDto filter)
        {
            var query = _dbSet.Include(t => t.AssignedUser).AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            if (filter.DueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= filter.DueDateFrom.Value);

            if (filter.DueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= filter.DueDateTo.Value);

            if (filter.AssignedUserId.HasValue)
                query = query.Where(t => t.AssignedUserId == filter.AssignedUserId.Value);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(t => t.Title.Contains(filter.SearchTerm) || t.Description.Contains(filter.SearchTerm));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(t => t.DueDate < now && t.Status != TaskItemStatus.Completed && t.Status != TaskItemStatus.Overdue)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(int userId)
            => await _dbSet.Where(t => t.AssignedUserId == userId).ToListAsync();

        public async Task<bool> ExistsAsync(int id)
            => await _dbSet.AnyAsync(t => t.Id == id);

        public async Task<int> GetTaskCountByStatusAsync(TaskItemStatus status)
            => await _dbSet.CountAsync(t => t.Status == status);
    }
}
