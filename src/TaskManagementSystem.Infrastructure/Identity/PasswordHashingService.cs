using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Domain;

namespace TaskManagementSystem.Infrastructure
{
    public class PasswordHashingService : IPasswordHasher<User>
    {
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }
}
