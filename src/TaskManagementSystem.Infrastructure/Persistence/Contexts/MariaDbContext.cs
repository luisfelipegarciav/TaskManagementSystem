using MySql.Data.MySqlClient;
using System.Data;

namespace TaskManagementSystem.Infrastructure.Persistence
{
    public class MariaDbContext : IDatabaseContext
    {
        private readonly string _connectionString;

        public MariaDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}
