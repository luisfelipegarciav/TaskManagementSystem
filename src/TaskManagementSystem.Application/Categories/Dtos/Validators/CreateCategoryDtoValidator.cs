using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(255).WithMessage("Category name must not exceed 255 characters");
        }
    }
}
