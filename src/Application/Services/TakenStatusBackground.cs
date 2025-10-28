using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGS.TaskTracker.Core.Enums;
using SGS.TaskTracker.Core.Interfaces;

namespace SGS.TaskTracker.Application.Services
{
    public class TaskStatusBackgroundService : BackgroundService
    {
        private readonly ILogger<TaskStatusBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval;

        public TaskStatusBackgroundService(
            ILogger<TaskStatusBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _interval = TimeSpan.FromHours(1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();

                    var overdueTasks = await taskRepository.GetOverdueTasksAsync();

                    foreach (var task in overdueTasks)
                    {
                        task.Status = TaskItemStatus.Overdue;
                        _logger.LogInformation("Marked task {TaskId} as overdue", task.Id);
                    }

                    if (overdueTasks.Any())
                    {
                        await taskRepository.SaveChangesAsync();
                        _logger.LogInformation("Updated {Count} tasks to overdue status", overdueTasks.Count());
                    }

                    await Task.Delay(_interval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating task statuses");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }
    }
}