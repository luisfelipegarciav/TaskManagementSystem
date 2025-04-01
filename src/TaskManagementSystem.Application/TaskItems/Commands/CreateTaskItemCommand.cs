using MediatR;

namespace TaskManagementSystem.Application
{
    public record CreateTaskItemCommand(int userId, CreateTaskItemDto dto) : IRequest<ServiceResponse<TaskItemDto>>;

    public class CreateTaskItemCommandHandler : IRequestHandler<CreateTaskItemCommand, ServiceResponse<TaskItemDto>>
    {
        private readonly ITaskItemService _taskItemService;

        public CreateTaskItemCommandHandler(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public async Task<ServiceResponse<TaskItemDto>> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
        {
            return await _taskItemService.CreateTaskItemAsync(request.userId, request.dto);
        }
    }
}
