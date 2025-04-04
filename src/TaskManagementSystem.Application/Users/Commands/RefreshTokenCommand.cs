using MediatR;

namespace TaskManagementSystem.Application
{
    public record RefreshTokenCommand(RefreshTokenRequestDto dto) : IRequest<ServiceResponse<TokenResponse>>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ServiceResponse<TokenResponse>>
    {
        private readonly IAuthenticationService _authenticationService;

        public RefreshTokenCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<ServiceResponse<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.RefreshTokenAsync(request.dto);
        }
    }
}
