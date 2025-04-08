using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.dto)
                .NotNull().WithMessage("User data is required.")
                .SetValidator(new UpdateUserDtoValidator());
        }
    }
}
