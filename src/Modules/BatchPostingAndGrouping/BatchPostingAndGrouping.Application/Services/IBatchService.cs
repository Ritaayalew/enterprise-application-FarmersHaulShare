using BatchPostingAndGrouping.Application.DTOs;
using BatchPostingAndGrouping.Domain.DomainEvents;

namespace BatchPostingAndGrouping.Application.Services;

public interface IBatchService
{
    Task<BatchDto> PostBatchAsync(Guid farmerProfileId, PostBatchDto dto, CancellationToken cancellationToken = default);
    Task<BatchDto?> GetBatchByIdAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BatchDto>> GetBatchesByFarmerAsync(Guid farmerProfileId, CancellationToken cancellationToken = default);
    Task<BatchDto> UpdateBatchAsync(Guid batchId, Guid farmerProfileId, UpdateBatchDto dto, CancellationToken cancellationToken = default);
    Task CancelBatchAsync(Guid batchId, Guid farmerProfileId, CancelBatchDto dto, CancellationToken cancellationToken = default);
}
