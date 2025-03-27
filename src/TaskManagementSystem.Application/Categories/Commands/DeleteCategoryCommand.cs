using MediatR;

namespace TaskManagementSystem.Application
{
    public record DeleteCategoryCommand(int id) : IRequest<ServiceResponse<bool>>;

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ServiceResponse<bool>>
    {
        private readonly ICategoryService _categoryService;

        public DeleteCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            return await _categoryService.DeleteCategoryByIdAsync(request.id);
        }
    }
}
