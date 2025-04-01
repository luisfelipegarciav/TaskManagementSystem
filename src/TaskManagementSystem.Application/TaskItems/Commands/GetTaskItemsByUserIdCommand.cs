using MediatR;

namespace TaskManagementSystem.Application
{
    public record GetTaskItemsByUserIdCommand(int userId, PaginationParamsDto pagination) : IRequest<ServiceResponse<PaginatedResultDto<TaskItemDto>>>;

    public class GetTaskItemsByUserIdCommandHandler : IRequestHandler<GetTaskItemsByUserIdCommand, ServiceResponse<PaginatedResultDto<TaskItemDto>>>
    {
        private readonly ITaskItemService _taskItemService;

        public GetTaskItemsByUserIdCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<ServiceResponse<PaginatedResultDto<TaskItemDto>>> Handle(GetTaskItemsByUserIdCommand request, CancellationToken cancellationToken)
        {
            return await _taskItemService.GetTaskItemsByUserIdAsync(request.userId, request.pagination);
        }
    }
}
