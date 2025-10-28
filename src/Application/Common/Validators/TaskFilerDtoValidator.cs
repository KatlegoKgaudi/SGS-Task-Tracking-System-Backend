using FluentValidation;
using SGS.TaskTracker.Core.DTOs;

namespace SGS.TaskTracker.Application.Common.Validators
{
    public class TaskFilterDtoValidator : AbstractValidator<TaskFilterDto>
    {
        public TaskFilterDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Invalid task status");

            RuleFor(x => x.DueDateFrom)
                .LessThan(x => x.DueDateTo).When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue)
                .WithMessage("Due date from must be before due date to");

            RuleFor(x => x.AssignedUserId)
                .GreaterThan(0).When(x => x.AssignedUserId.HasValue)
                .WithMessage("Assigned user ID must be a positive number");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.SearchTerm))
                .WithMessage("Search term must not exceed 100 characters");
        }
    }
}
