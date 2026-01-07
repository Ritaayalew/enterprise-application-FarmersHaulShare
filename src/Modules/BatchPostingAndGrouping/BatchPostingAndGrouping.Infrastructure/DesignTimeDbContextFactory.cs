using Microsoft.EntityFrameworkCore;
using BatchPostingAndGrouping.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;

namespace FarmersHaulShare.BatchPostingAndGrouping.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BatchPostingAndGroupingDbContext>
{
    public BatchPostingAndGroupingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BatchPostingAndGroupingDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=farmershaulshare;Username=admin;Password=password");

        return new BatchPostingAndGroupingDbContext(optionsBuilder.Options);
    }
}
}
