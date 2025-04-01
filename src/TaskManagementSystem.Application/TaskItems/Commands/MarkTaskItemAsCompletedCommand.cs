using MediatR;

namespace TaskManagementSystem.Application
{
    public record MarkTaskItemAsCompletedCommand(int userId, int taskId) : IRequest<ServiceResponse<bool>>;

    public class MarkTaskItemAsCompletedCommandHandler : IRequestHandler<MarkTaskItemAsCompletedCommand, ServiceResponse<bool>>
    {
        private readonly ITaskItemService _taskItemService;

        public MarkTaskItemAsCompletedCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<ServiceResponse<bool>> Handle(MarkTaskItemAsCompletedCommand request, CancellationToken cancellationToken)
        {
            return await _taskItemService.MarkTaskItemAsCompletedAsync(request.taskId, request.userId);
        }
    }
}
