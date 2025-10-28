using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Core.DTOs
{
    public class TaskCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.New;
        public DateTime DueDate { get; set; }
        public int? AssignedUserId { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
