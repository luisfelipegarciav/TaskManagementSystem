using MediatR;

namespace TaskManagementSystem.Application
{
    public record UpdateTaskItemCommand(int userId, int taskId, UpdateTaskItemDto dto) : IRequest<ServiceResponse<bool>>;

    public class UpdateTaskItemCommandHandler : IRequestHandler<UpdateTaskItemCommand, ServiceResponse<bool>>
    {
        private readonly ITaskItemService _taskItemService;

        public UpdateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<ServiceResponse<bool>> Handle(UpdateTaskItemCommand request, CancellationToken cancellationToken)
        {
            return await _taskItemService.UpdateTaskItemAsync(request.taskId, request.userId, request.dto);
        }
    }
}
