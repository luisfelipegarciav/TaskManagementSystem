using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TaskManagementSystem.Infrastructure.Persistence
{
    public class SqlServerContext : IDatabaseContext
    {
        private readonly string _connectionString;

        public SqlServerContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SqlServerConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
