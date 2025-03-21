using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace TaskManagementSystem.Infrastructure.Persistence
{
    public class MariaDbContext : IDatabaseContext
    {
        private readonly string _connectionString;

        public MariaDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MariaDbConnection") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
        }

        public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
    }
}
