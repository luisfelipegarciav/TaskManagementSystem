namespace TaskManagementSystem.Application.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message = "Entity not found.") : base(message) { }
    }
}
