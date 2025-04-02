namespace TaskManagementSystem.Application
{
    public class TaskItemNotFoundException : Exception
    {
        public TaskItemNotFoundException(string message = "Task Item not found") : base(message) { }
    }
}
