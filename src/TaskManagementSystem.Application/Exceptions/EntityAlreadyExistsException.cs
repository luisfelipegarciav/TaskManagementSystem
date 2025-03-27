namespace TaskManagementSystem.Application
{
    public class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException(string message = "Entity already exists.") : base(message) {}
    }
}
