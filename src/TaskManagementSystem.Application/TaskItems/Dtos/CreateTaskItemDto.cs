using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Application
{
    public class CreateTaskItemDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public int CategoryId { get; set; }
    }
}
