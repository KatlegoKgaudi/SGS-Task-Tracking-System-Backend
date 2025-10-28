using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Core.DTOs
{
    public class TaskUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskItemStatus? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssignedUserId { get; set; }
    }
}
