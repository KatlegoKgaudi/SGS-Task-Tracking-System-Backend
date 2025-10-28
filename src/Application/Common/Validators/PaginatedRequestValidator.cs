using FluentValidation;
using SGS.TaskTracker.Core.DTOs;

namespace SGS.TaskTracker.Application.Common.Validators
{
    public class PaginatedRequestValidator : AbstractValidator<PaginatedRequest>
    {
        public PaginatedRequestValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SortBy)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.SortBy))
                .WithMessage("Sort by field must not exceed 50 characters");
        }
    }
}
