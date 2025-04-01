using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Domain;
using TaskManagementSystem.Infrastructure.Identity;
using TaskManagementSystem.Infrastructure.Persistence;
using TaskManagementSystem.Infrastructure.Persistence.Repositories;

namespace TaskManagementSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration); // Register persistence services
            services.AddIdentityInfrastructure(configuration); // register identity services.

            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRepository<Category>, CategoryRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IRepository<TaskItem>, TaskItemRepository>();
            services.AddScoped<ITaskItemRepository, TaskItemRepository>();

            // Add other infrastructure services here (e.g., email service, logging)
            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context based on configuration
            string? databaseProvider = string.IsNullOrEmpty(configuration["DatabaseProvider"]) ? Environment.GetEnvironmentVariable("TskMgr_DatabaseProvider") : configuration["DatabaseProvider"];
            string? sqlServerConnectionString = string.IsNullOrEmpty(configuration["ConnectionStrings:SqlServerConnection"]) ? Environment.GetEnvironmentVariable("TskMgr_ConnectionStrings__SqlServerConnection") : configuration["ConnectionStrings:SqlServerConnection"];
            string? mariaDbConnectionString = string.IsNullOrEmpty(configuration["ConnectionStrings:MariaDbConnection"]) ? Environment.GetEnvironmentVariable("TskMgr_ConnectionStrings__MariaDbConnection") : configuration["ConnectionStrings:MariaDbConnection"];

            if (string.IsNullOrWhiteSpace(databaseProvider))
            {
                throw new ArgumentNullException("DatabaseProvider setting is required.");
            }

            switch (databaseProvider?.ToLower())
            {
                case "sqlserver":
                    if (string.IsNullOrWhiteSpace(sqlServerConnectionString))
                        throw new ArgumentNullException("SqlServerConnection setting is required.");
                    services.AddScoped<IDatabaseContext, SqlServerContext>(provider => new SqlServerContext(sqlServerConnectionString));
                    break;
                default:
                    if (string.IsNullOrWhiteSpace(mariaDbConnectionString))
                        throw new ArgumentNullException("MariaDbConnection setting is required.");
                    services.AddScoped<IDatabaseContext, MariaDbContext>(provider => new MariaDbContext(mariaDbConnectionString)); // Default to MariaDb
                    break;
            }

            return services;
        }
    }
}
