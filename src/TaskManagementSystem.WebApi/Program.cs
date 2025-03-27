using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Json;
using System.Text;
using TaskManagementSystem.Application;
using TaskManagementSystem.Infrastructure;
using TaskManagementSystem.Infrastructure.Identity;
using TaskManagementSystem.WebApi.Filters;
using TaskManagementSystem.WebApi.Middleware;

namespace TaskManagementSystem.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add appsettings.json configuration provider
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddEnvironmentVariables();

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
                // Add JWT Bearer Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Register the new filter
                c.OperationFilter<AddAuthorizationHeaderOperationFilter>();
            });

            // Configure JWT Bearer Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = string.IsNullOrEmpty(builder.Configuration["Jwt:Issuer"]) ? Environment.GetEnvironmentVariable("TskMgr_Jwt__Issuer") : builder.Configuration["Jwt:Issuer"],
                        ValidAudience = string.IsNullOrEmpty(builder.Configuration["Jwt:Audience"]) ? Environment.GetEnvironmentVariable("TskMgr_Jwt__Audience") : builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(string.IsNullOrEmpty(builder.Configuration["Jwt:SecretKey"]) ? Environment.GetEnvironmentVariable("TskMgr_Jwt__SecretKey") : builder.Configuration["Jwt:SecretKey"]))
                    };
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

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
