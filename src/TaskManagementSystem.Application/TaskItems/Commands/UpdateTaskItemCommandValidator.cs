using FluentValidation;

namespace TaskManagementSystem.Application
{
    public class UpdateTaskItemCommandValidator : AbstractValidator<UpdateTaskItemCommand>
    {
        public UpdateTaskItemCommandValidator()
        {
            RuleFor(x => x.dto)
                .NotNull().WithMessage("Task item data is required.")
                .SetValidator(new CreateTaskItemDtoValidator());
        }
    }
}
