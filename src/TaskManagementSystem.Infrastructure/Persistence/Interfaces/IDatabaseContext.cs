using System.Data;

namespace TaskManagementSystem.Infrastructure.Persistence
{
    public interface IDatabaseContext
    {
        IDbConnection CreateConnection();
    }
}
