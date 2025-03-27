using System.Data;
using System.Data.SqlClient;

namespace TaskManagementSystem.Infrastructure.Persistence
{
    public class SqlServerContext : IDatabaseContext
    {
        private readonly string _connectionString;

        public SqlServerContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
