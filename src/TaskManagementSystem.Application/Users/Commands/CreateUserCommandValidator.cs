using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.dto)
                .NotNull().WithMessage("User data is required.")
                .SetValidator(new CreateUserDtoValidator());
        }
    }
}
