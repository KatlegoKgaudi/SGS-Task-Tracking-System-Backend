namespace SGS.TaskTracker.Core.DTOs
{
    public class TaskSummaryDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int InProgressTasks { get; set; }
    }
}
