using MediatR;

namespace TaskManagementSystem.Application
{
    public record CreateUserCommand(CreateUserDto dto) : IRequest<ServiceResponse<UserDto>>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ServiceResponse<UserDto>>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ServiceResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateUserAsync(request.dto);
        }
    }

}
