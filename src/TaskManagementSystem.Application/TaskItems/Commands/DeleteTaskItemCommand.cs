using MediatR;

namespace TaskManagementSystem.Application.TaskItems.Commands
{
    public record DeleteTaskItemCommand(int userId, int taskId) : IRequest<ServiceResponse<bool>>;

    public class DeleteTaskItemCommandHandler : IRequestHandler<DeleteTaskItemCommand, ServiceResponse<bool>>
    {
        private readonly ITaskItemService _taskItemService;

        public DeleteTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteTaskItemCommand request, CancellationToken cancellationToken)
        {
            return await _taskItemService.DeleteTaskItemAsync(request.taskId, request.userId);
        }
    }
}
