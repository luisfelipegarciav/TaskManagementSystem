using MediatR;

namespace TaskManagementSystem.Application
{
    public record GetCategoryCommand() : IRequest<ServiceResponse<List<CategoryDto>>>;

    public class GetCategoryCommandHandler : IRequestHandler<GetCategoryCommand, ServiceResponse<List<CategoryDto>>>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<ServiceResponse<List<CategoryDto>>> Handle(GetCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryService.GetAllCategoriesAsync();
        }
    }
}
