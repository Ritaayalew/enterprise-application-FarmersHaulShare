using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityAndAccessManagement.Infrastructure.DbContexts;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

        // Use your real connection string (update username/password)
        // For schema isolation: add SearchPath=iam
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=farmershaulshare;Username=postgres;Password=yourpassword;SearchPath=iam"
        );

        // Optional: Show detailed errors during migrations
        // optionsBuilder.EnableSensitiveDataLogging();
        // optionsBuilder.EnableDetailedErrors();

        return new IdentityDbContext(optionsBuilder.Options);
    }
}