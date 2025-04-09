using MediatR;

namespace TaskManagementSystem.Application
{
    public record UpdateUserCommand(UpdateUserDto dto) : IRequest<ServiceResponse<bool>>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ServiceResponse<bool>>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ServiceResponse<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.UpdateUserAsync(request.dto);
        }
    }
}
