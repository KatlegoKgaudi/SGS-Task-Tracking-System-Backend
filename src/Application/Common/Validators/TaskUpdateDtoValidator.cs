using FluentValidation;
using SGS.TaskTracker.Core.DTOs;

namespace SGS.TaskTracker.Application.Common_.Validators
{
    public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
    {
        public TaskUpdateDtoValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Title))
                .WithMessage("Title must not exceed 200 characters")
                .MinimumLength(3).When(x => !string.IsNullOrEmpty(x.Title))
                .WithMessage("Title must be at least 3 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).When(x => x.DueDate.HasValue)
                .WithMessage("Due date must be in the future");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Invalid task status");

            RuleFor(x => x.AssignedUserId)
                .GreaterThan(0).When(x => x.AssignedUserId.HasValue)
                .WithMessage("Assigned user ID must be a positive number");
        }
    }
}
