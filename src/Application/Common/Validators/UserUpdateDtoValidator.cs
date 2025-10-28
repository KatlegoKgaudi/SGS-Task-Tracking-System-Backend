using FluentValidation;
using SGS.TaskTracker.Core.DTOs;

namespace SGS.TaskTracker.Application.Common.Validators
{
    public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(x => x.Username)
                .MinimumLength(3).When(x => !string.IsNullOrEmpty(x.Username))
                .WithMessage("Username must be at least 3 characters")
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Username))
                .WithMessage("Username must not exceed 50 characters")
                .Matches("^[a-zA-Z0-9_]+$").When(x => !string.IsNullOrEmpty(x.Username))
                .WithMessage("Username can only contain letters, numbers, and underscores");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("A valid email address is required")
                .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("Email must not exceed 255 characters");

            RuleFor(x => x.Role)
                .IsInEnum().When(x => x.Role.HasValue)
                .WithMessage("Invalid user role");
        }
    }
}
