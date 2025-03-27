using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.dto)
                .NotNull().WithMessage("Category data is required.")
                .SetValidator(new CreateCategoryDtoValidator());
        }
    }
}
