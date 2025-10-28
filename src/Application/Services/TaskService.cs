using SGS.TaskTracker.Application.Common.Mappings;
using SGS.TaskTracker.Core.DTOs;
using SGS.TaskTracker.Core.Enums;
using SGS.TaskTracker.Core.Interfaces;
using SGS.TaskTracker.Interfaces;

namespace SGS.TaskTracker.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            return tasks.ToTaskResponseDtos();
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksWithFiltersAsync(TaskFilterDto filter)
        {
            var tasks = await _taskRepository.GetTasksWithFiltersAsync(filter);
            return tasks.ToTaskResponseDtos();
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.AssignedUserId != userId)
                return null;

            return task.ToTaskResponseDto();
        }

        public async Task<TaskResponseDto> CreateTaskAsync(TaskCreateDto taskCreateDto)
        {
            if (taskCreateDto.AssignedUserId.HasValue)
            {
                var userExists = await _userRepository.GetByIdAsync(taskCreateDto.AssignedUserId.Value) != null;
                if (!userExists)
                {
                    throw new ArgumentException("Assigned user does not exist");
                }
            }

            var task = taskCreateDto.ToTaskItem();
            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            var createdTask = await _taskRepository.GetByIdAsync(task.Id);
            return createdTask!.ToTaskResponseDto();
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int id, TaskUpdateDto taskUpdateDto, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.AssignedUserId != userId)
                return null;

            if (taskUpdateDto.AssignedUserId.HasValue)
            {
                var userExists = await _userRepository.GetByIdAsync(taskUpdateDto.AssignedUserId.Value) != null;
                if (!userExists)
                {
                    throw new ArgumentException("Assigned user does not exist");
                }
            }

            task.UpdateFromDto(taskUpdateDto);
            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();

            var updatedTask = await _taskRepository.GetByIdAsync(id);
            return updatedTask!.ToTaskResponseDto();
        }

        public async Task<bool> DeleteTaskAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null || task.AssignedUserId != userId)
                return false;

            _taskRepository.Delete(task);
            return await _taskRepository.SaveChangesAsync() > 0;
        }

        public async Task<TaskSummaryDto> GetTaskSummaryAsync(int userId)
        {
            var userTasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            var totalTasks = userTasks.Count();
            var completedTasks = userTasks.Count(t => t.Status == TaskItemStatus.Completed);
            var overdueTasks = userTasks.Count(t => t.Status == TaskItemStatus.Overdue);
            var inProgressTasks = userTasks.Count(t => t.Status == TaskItemStatus.InProgress);

            return new TaskSummaryDto
            {
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                InProgressTasks = inProgressTasks
            };
        }

        public async Task<bool> AssignTaskAsync(int taskId, int assignToUserId, int currentUserId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            var user = await _userRepository.GetByIdAsync(assignToUserId);

            if (task == null || task.AssignedUserId != currentUserId || user == null)
                return false;

            task.AssignedUserId = assignToUserId;
            _taskRepository.Update(task);
            return await _taskRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus status, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.AssignedUserId != userId)
                return false;

            task.Status = status;
            _taskRepository.Update(task);
            return await _taskRepository.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            return tasks.ToTaskResponseDtos();
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksAssignedToUserAsync(int userId)
        {
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);
            return tasks.ToTaskResponseDtos();
        }

        public async Task<bool> TaskExistsAsync(int id)
        {
            return await _taskRepository.ExistsAsync(id);
        }
    }

}

