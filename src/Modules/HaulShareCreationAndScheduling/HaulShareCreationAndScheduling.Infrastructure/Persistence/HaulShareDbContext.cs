using HaulShareCreationAndScheduling.Domain.Aggregates;
using HaulShareCreationAndScheduling.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaulShareCreationAndScheduling.Infrastructure.Persistence;

public class HaulShareDbContext : DbContext
{
    public DbSet<HaulShare> HaulShares => Set<HaulShare>();
    public DbSet<PickupStop> PickupStops => Set<PickupStop>();

    public HaulShareDbContext(DbContextOptions<HaulShareDbContext> options)
        : base(options)
    {
    }
}
