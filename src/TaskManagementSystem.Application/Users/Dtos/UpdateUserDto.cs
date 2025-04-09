namespace TaskManagementSystem.Application
{
    public class UpdateUserDto
    {
        public int? Id { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
