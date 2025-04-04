namespace TaskManagementSystem.Application
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message = "An unexpected error happened on authentication.") : base(message) { }
    }
}
