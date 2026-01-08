using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CatalogAndContracts.Infrastructure.Persistence;
using CatalogAndContracts.Infrastructure.Repositories;
using CatalogAndContracts.Infrastructure.AIIntegration;

namespace CatalogAndContracts.Infrastructure.Config
{
    public static class CatalogModuleConfig
    {
        public static IServiceCollection AddCatalogModuleInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<ContractRepository>();
            services.AddScoped<ContractAIService>();

            return services;
        }
    }
}