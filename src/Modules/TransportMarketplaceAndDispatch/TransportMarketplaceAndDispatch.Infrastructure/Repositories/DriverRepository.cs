using Microsoft.EntityFrameworkCore;
using TransportMarketplaceAndDispatch.Domain.Aggregates;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Infrastructure.Persistence;

namespace TransportMarketplaceAndDispatch.Infrastructure.Repositories;

public sealed class DriverRepository : IDriverRepository
{
    private readonly TransportDbContext _dbContext;

    public DriverRepository(TransportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Drivers
            .Include(d => d.Vehicles)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<Driver?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Drivers
            .Include(d => d.Vehicles)
            .FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<Driver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Drivers
            .Include(d => d.Vehicles)
            .Where(d => d.IsActive && d.Vehicles.Any(v => v.IsVerified))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        await _dbContext.Drivers.AddAsync(driver, cancellationToken);
    }

    public async Task UpdateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        _dbContext.Drivers.Update(driver);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}