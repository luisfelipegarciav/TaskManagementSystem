using MediatR;

namespace TaskManagementSystem.Application
{
    public record DeleteUserCommand(int userId) : IRequest<ServiceResponse<bool>>;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ServiceResponse<bool>>
    {
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.DeleteUserByIdAsync(request.userId);
        }
    }
}
