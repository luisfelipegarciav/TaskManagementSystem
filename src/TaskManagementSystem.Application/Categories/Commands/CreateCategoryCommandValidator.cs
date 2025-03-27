using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.dto)
                .NotNull().WithMessage("Category data is required.")
                .SetValidator(new CreateCategoryDtoValidator());
        }
    }
}
