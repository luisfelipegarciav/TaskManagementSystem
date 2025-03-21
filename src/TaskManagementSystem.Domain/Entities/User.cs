namespace TaskManagementSystem.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
