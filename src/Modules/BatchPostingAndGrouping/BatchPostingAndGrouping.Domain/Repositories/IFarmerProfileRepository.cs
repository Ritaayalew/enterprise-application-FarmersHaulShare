using BatchPostingAndGrouping.Domain.Entities;

namespace BatchPostingAndGrouping.Domain.Repositories;

public interface IFarmerProfileRepository
{
    Task<FarmerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FarmerProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmerProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(FarmerProfile farmerProfile, CancellationToken cancellationToken = default);
    Task UpdateAsync(FarmerProfile farmerProfile, CancellationToken cancellationToken = default);
}
