using MediatR;

namespace TaskManagementSystem.Application
{
    public record GetTaskItemCommand(int userId, int taskId) : IRequest<ServiceResponse<TaskItemDto>>;

    public class GetTaskItemCommandHandler : IRequestHandler<GetTaskItemCommand, ServiceResponse<TaskItemDto>>
    {
        private readonly ITaskItemService _taskItemService;

        public GetTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<ServiceResponse<TaskItemDto>> Handle(GetTaskItemCommand request, CancellationToken cancellationToken)
        {
            return await _taskItemService.GetTaskItemByIdAsync(request.taskId, request.userId);
        }
    }
}
