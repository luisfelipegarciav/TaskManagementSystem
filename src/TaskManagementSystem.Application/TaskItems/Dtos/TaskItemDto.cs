using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
