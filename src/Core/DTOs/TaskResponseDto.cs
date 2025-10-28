using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Core.DTOs
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }
    }
}
