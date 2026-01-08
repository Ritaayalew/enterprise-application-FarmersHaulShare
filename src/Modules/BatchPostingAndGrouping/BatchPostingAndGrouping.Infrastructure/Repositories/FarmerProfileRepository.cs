using BatchPostingAndGrouping.Domain.Entities;
using BatchPostingAndGrouping.Domain.Repositories;
using BatchPostingAndGrouping.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BatchPostingAndGrouping.Infrastructure.Repositories;

public sealed class FarmerProfileRepository : IFarmerProfileRepository
{
    private readonly BatchPostingDbContext _context;

    public FarmerProfileRepository(BatchPostingDbContext context)
    {
        _context = context;
    }

    public async Task<FarmerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FarmerProfiles
            .Include(f => f.Batches)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<FarmerProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Note: This assumes a UserId field exists. For now, we'll use Name or PhoneNumber
        // You may need to adjust this based on your identity management integration
        return await _context.FarmerProfiles
            .FirstOrDefaultAsync(f => f.Name == userId || f.PhoneNumber == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<FarmerProfile>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FarmerProfiles
            .OrderBy(f => f.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FarmerProfile farmerProfile, CancellationToken cancellationToken = default)
    {
        await _context.FarmerProfiles.AddAsync(farmerProfile, cancellationToken);
    }

    public async Task UpdateAsync(FarmerProfile farmerProfile, CancellationToken cancellationToken = default)
    {
        _context.FarmerProfiles.Update(farmerProfile);
        await Task.CompletedTask;
    }
}
