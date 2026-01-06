using HaulShareCreationAndScheduling.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace HaulShareCreationAndScheduling.Infrastructure.Persistence;

public class HaulShareDbContext : DbContext
{
    public DbSet<HaulShare> HaulShares => Set<HaulShare>();

    public HaulShareDbContext(DbContextOptions<HaulShareDbContext> options)
        : base(options)
    {
    }
}
