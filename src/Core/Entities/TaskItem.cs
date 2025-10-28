using SGS.TaskTracker.Core.Enums;

namespace SGS.TaskTracker.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskItemStatus Status { get; set; } = TaskItemStatus.New;
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public int? AssignedUserId { get; set; }
        public virtual User? AssignedUser { get; set; }
    }
}
