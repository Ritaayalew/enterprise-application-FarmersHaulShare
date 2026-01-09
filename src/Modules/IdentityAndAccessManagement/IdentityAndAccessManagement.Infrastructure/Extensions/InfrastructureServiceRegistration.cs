using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityAndAccessManagement.Infrastructure.DbContexts;
using IdentityAndAccessManagement.Application.Abstractions;
using IdentityAndAccessManagement.Infrastructure.Persistence;
using IdentityAndAccessManagement.Infrastructure.Repositories;

namespace IdentityAndAccessManagement.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("IdentityConnection");
            options.UseNpgsql(connectionString, b => b.MigrationsHistoryTable("__EFMigrationsHistory", "iam"));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
