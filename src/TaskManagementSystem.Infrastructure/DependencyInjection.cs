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

            // Add other infrastructure services here (e.g., email service, logging)
            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context based on configuration
            string databaseProvider = configuration["DatabaseProvider"];

            switch (databaseProvider?.ToLower())
            {
                case "sqlserver":
                    services.AddScoped<IDatabaseContext, SqlServerContext>();
                    break;
                default:
                    services.AddScoped<IDatabaseContext, MariaDbContext>(); // Default to MariaDb
                    break;
            }

            return services;
        }
    }
}
