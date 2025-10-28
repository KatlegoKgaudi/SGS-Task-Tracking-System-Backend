using FluentValidation;
using SGS.TaskTracker.Core.DTOs;

namespace SGS.TaskTracker.Application.Common.Validators
{
    public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
    {
        public TaskCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future");

            RuleFor(x => x.AssignedUserId)
                .GreaterThan(0).When(x => x.AssignedUserId.HasValue)
                .WithMessage("Assigned user ID must be a positive number");
        }
    }
}
