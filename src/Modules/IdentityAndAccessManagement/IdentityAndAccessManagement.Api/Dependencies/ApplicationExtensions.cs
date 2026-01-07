using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using IdentityAndAccessManagement.Application;

namespace IdentityAndAccessManagement.Api.Dependencies;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationLayer(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(AssemblyRef).Assembly);
        });

        return services;
    }
}
