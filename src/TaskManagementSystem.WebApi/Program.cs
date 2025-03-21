using TaskManagementSystem.Application;
using TaskManagementSystem.Infrastructure;
using TaskManagementSystem.Infrastructure.Identity;
using TaskManagementSystem.WebApi.Middleware;
using Serilog;
using Serilog.Formatting.Json;

namespace TaskManagementSystem.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Set the minimum log level
                .WriteTo.File(
                    path: "logs/myapp-.json",
                    rollingInterval: RollingInterval.Day, // Roll daily
                    fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB limit
                    rollOnFileSizeLimit: true, // Roll on size limit
                    retainedFileCountLimit: 31, // Retain 31 days of logs
                    formatter: new JsonFormatter() // Use JSON formatter
                ) // Configure file sink
                .CreateLogger();

            builder.Host.UseSerilog(); // Use Serilog as the logging provider

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Task Management API",
                    Description = "An API for managing tasks"
                });
            });

            // Add Application and Infrastructure dependencies
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            // Add Identity Infrastructure
            builder.Services.AddIdentityInfrastructure(builder.Configuration); // Call the extension method

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
