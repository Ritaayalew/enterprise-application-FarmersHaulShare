using TransportMarketplaceAndDispatch.Application.DTOs;

namespace TransportMarketplaceAndDispatch.Application.Queries;

public sealed class GetJobStatusQuery
{
    public Guid DispatchJobId { get; init; }
}

public sealed class GetJobStatusResult
{
    public DispatchJobDto? Job { get; init; }
}