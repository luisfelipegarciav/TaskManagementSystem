using MediatR;

namespace TaskManagementSystem.Application
{
    public record GetUsersCommand(PaginationParamsDto pagination) : IRequest<ServiceResponse<PaginatedResultDto<UserDto>>>;

    public class GetUsersCommandHandler : IRequestHandler<GetUsersCommand, ServiceResponse<PaginatedResultDto<UserDto>>>
    {
        private readonly IUserService _userService;

        public GetUsersCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ServiceResponse<PaginatedResultDto<UserDto>>> Handle(GetUsersCommand request, CancellationToken cancellationToken)
        {
            return await _userService.GetUsersAsync(request.pagination);
        }
    }
}
