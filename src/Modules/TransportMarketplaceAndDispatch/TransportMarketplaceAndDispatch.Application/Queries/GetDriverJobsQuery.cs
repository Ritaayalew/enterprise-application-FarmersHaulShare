using TransportMarketplaceAndDispatch.Application.DTOs;

namespace TransportMarketplaceAndDispatch.Application.Queries;

public sealed class GetDriverJobsQuery
{
    public Guid DriverId { get; init; }
    public bool IncludeCompleted { get; init; } = false;
}

public sealed class GetDriverJobsResult
{
    public List<DispatchJobDto> Jobs { get; init; } = new();
}