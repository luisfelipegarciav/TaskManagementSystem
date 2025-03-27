using MediatR;

namespace TaskManagementSystem.Application
{
    public record UpdateCategoryCommand(int id, UpdateCategoryDto dto) : IRequest<ServiceResponse<bool>>;

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ServiceResponse<bool>>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<ServiceResponse<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryService.UpdateCategoryAsync(request.id, request.dto);
        }
    }
}
