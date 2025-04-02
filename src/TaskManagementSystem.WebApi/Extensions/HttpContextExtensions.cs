using System.Security.Claims;

namespace TaskManagementSystem.WebApi.Extensions
{
    public static class HttpContextExtensions
    {
        public static int? GetUserId(this HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return null;
            }
            return userId;
        }
    }
}
