namespace TaskManagementSystem.Domain
{
    public class TaskItem
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
    }
}
