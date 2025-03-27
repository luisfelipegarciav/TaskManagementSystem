using MediatR;

namespace TaskManagementSystem.Application
{
    public record CreateCategoryCommand(CreateCategoryDto dto) : IRequest<ServiceResponse<CategoryDto>>;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ServiceResponse<CategoryDto>>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<ServiceResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryService.CreateCategoryAsync(request.dto);
        }
    }
}
