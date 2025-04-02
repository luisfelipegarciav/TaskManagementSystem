using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagementSystem.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);
            services.AddValidatorsFromAssemblyContaining<CreateUserDto>();
            services.AddValidatorsFromAssemblyContaining<CreateCategoryDto>();
            services.AddValidatorsFromAssemblyContaining<CreateTaskItemDto>();
            services.AddValidatorsFromAssemblyContaining<UpdateTaskItemDto>();
            services.AddValidatorsFromAssemblyContaining<AuthenticateUserCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ITaskItemService, TaskItemService>();
            return services;
        }   
    }
}
