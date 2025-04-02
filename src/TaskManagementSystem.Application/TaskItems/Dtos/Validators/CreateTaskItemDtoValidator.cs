using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class CreateTaskItemDtoValidator : AbstractValidator<CreateTaskItemDto>
    {
        public CreateTaskItemDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(255).WithMessage("Title must not exceed 255 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(255).WithMessage("Description must not exceed 255 characters");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Date is required");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required");
        }
    }
}
