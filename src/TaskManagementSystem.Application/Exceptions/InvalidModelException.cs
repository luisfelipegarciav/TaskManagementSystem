namespace TaskManagementSystem.Application
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException(string message = "Invalid model.") : base(message) { }
    }
}
