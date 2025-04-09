using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("User ID is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("Roles are required.");
        }
    }
}
