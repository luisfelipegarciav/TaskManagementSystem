namespace TaskManagementSystem.Application
{
    public class UpdateEntityException : Exception
    {
        public UpdateEntityException(string message = "An exception happened while updating the record.") : base(message) { }
    }
}
