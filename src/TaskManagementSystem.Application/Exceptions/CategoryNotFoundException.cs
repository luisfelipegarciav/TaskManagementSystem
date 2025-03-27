namespace TaskManagementSystem.Application
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException(string message = "Category not found") : base(message) { }
    }
}
