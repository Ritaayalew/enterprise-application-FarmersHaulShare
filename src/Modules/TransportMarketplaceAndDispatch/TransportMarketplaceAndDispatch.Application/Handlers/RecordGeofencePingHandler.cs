using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class RecordGeofencePingHandler
{
    private readonly IDispatchJobRepository _dispatchJobRepository;

    public RecordGeofencePingHandler(IDispatchJobRepository dispatchJobRepository)
    {
        _dispatchJobRepository = dispatchJobRepository;
    }

    public async Task Handle(RecordGeofencePingCommand command, CancellationToken cancellationToken)
    {
        var job = await _dispatchJobRepository.GetByIdAsync(command.DispatchJobId, cancellationToken);
        if (job == null)
            throw new InvalidOperationException($"Dispatch job with ID {command.DispatchJobId} not found");

        job.RecordGeofencePing(command.Latitude, command.Longitude, command.PingTime);

        await _dispatchJobRepository.UpdateAsync(job, cancellationToken);
        await _dispatchJobRepository.SaveChangesAsync(cancellationToken);
    }
}