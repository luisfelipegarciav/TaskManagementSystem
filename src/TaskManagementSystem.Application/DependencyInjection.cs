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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient<IUserService, UserService>();
            return services;
        }   
    }
}
