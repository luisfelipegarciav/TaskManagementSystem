using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse<TokenResponse>> AuthenticateAsync(string username, string password);
    }
}
