using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Core.DTOs
{
    public class TaskFilterDto
    {
        public string? SearchTerm { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public TaskItemStatus? Status { get; set; }
        public int? AssignedUserId { get; set; }
        public int UserId { get; set; }
    }
}
