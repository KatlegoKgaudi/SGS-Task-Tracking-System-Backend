using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGS.TaskTracker.Enums;
using System.Security.Claims;

namespace SGS.TaskTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _tasksService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasks([FromQuery] TaskFilterDto? filter = null)
        {
            var userId = GetCurrentUserId();

            if (filter == null)
            {
                var tasks = await _taskService.GetAllTasksAsync(userId);
                return Ok(tasks);
            }

            filter.UserId = userId;
            var filteredTasks = await _taskService.GetTasksWithFiltersAsync(filter);
            return Ok(filteredTasks);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.GetTaskByIdAsync(id, userId);
            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost("CreateTask")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] TaskCreateDto taskCreateDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                taskCreateDto.CreatedByUserId = userId;

                if (taskCreateDto.AssignedUserId == null)
                {
                    taskCreateDto.AssignedUserId = userId;
                }

                var task = await _taskService.CreateTaskAsync(taskCreateDto);
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPut("{taskId}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(int taskId, [FromBody] TaskUpdateDto taskUpdateDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var task = await _taskService.UpdateTaskAsync(taskId, taskUpdateDto, userId);
                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{taskId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteTask(int taskId)
        {
            var userId = GetCurrentUserId();
            var deleted = await _taskService.DeleteTaskAsync(taskId, userId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(TaskSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<TaskSummaryDto>> GetTaskSummary()
        {
            var userId = GetCurrentUserId();
            var summary = await _taskService.GetTaskSummaryAsync(userId);
            return Ok(summary);
        }

        [HttpPost("{id}/assign/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignTask(int id, int userId)
        {
            var currentUserId = GetCurrentUserId();
            var assigned = await _taskService.AssignTaskAsync(id, userId, currentUserId);
            if (!assigned)
                return NotFound(new { message = "Task or user not found" });

            return Ok(new { message = "Task assigned successfully" });
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> UpdateTaskStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var userId = GetCurrentUserId();
            var updated = await _taskService.UpdateTaskStatusAsync(id, request.Status, userId);
            if (!updated)
                return NotFound();

            return Ok(new { message = "Task status updated successfully" });
        }

        [HttpGet("my-tasks")]
        [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetMyTasks()
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("assigned-to-me")]
        [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksAssignedToMe()
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetTasksAssignedToUserAsync(userId);
            return Ok(tasks);
        }
    }
}
public class UpdateStatusRequest
{
    public TaskItemStatus Status { get; set; }
}

