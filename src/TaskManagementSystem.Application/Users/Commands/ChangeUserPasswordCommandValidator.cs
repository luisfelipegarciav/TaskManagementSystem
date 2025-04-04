using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        public ChangeUserPasswordCommandValidator()
        {
            RuleFor(x => x.dto)
                .NotNull().WithMessage("Required fields missing");

            RuleFor(x => x.dto.CurrentPassword)
                .NotNull().WithMessage("Current password is required");

            RuleFor(x => x.dto.NewPassword)
                .NotNull().WithMessage("New password is required")
                .NotEqual(x => x.dto.CurrentPassword)
                .WithMessage("New password can not be same as current password.");
        }
    }
}
