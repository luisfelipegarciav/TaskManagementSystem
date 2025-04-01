using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class CreateTaskItemCommandValidator : AbstractValidator<CreateTaskItemCommand>
    {
        public CreateTaskItemCommandValidator()
        {
            RuleFor(x => x.dto)
                .NotNull().WithMessage("Category data is required.")
                .SetValidator(new CreateTaskItemDtoValidator());
        }
    }
}
