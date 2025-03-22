using MediatR;

namespace TaskManagementSystem.Application
{
    public class AuthenticateUserCommand : IRequest<ServiceResponse<TokenResponse>>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public AuthenticateUserCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, ServiceResponse<TokenResponse>>
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticateUserCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<ServiceResponse<TokenResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.AuthenticateAsync(request.Username, request.Password);
        }
    }
}
