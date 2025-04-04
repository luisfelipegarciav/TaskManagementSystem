using MediatR;

namespace TaskManagementSystem.Application
{
    public record ChangeUserPasswordCommand(int userId, ChangePasswordRequestDto dto) : IRequest<ServiceResponse<bool>>;

    public class ChangePasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, ServiceResponse<bool>>
    {
        private readonly IUserService _userService;

        public ChangePasswordCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ServiceResponse<bool>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _userService.ChangePasswordAsync(request.userId, request.dto);
        }
    }
}
