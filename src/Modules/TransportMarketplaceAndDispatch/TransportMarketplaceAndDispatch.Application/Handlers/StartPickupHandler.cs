using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class StartPickupHandler
{
    private readonly IDispatchJobRepository _dispatchJobRepository;

    public StartPickupHandler(IDispatchJobRepository dispatchJobRepository)
    {
        _dispatchJobRepository = dispatchJobRepository;
    }

    public async Task Handle(StartPickupCommand command, CancellationToken cancellationToken)
    {
        var job = await _dispatchJobRepository.GetByIdAsync(command.DispatchJobId, cancellationToken);
        if (job == null)
            throw new InvalidOperationException($"Dispatch job with ID {command.DispatchJobId} not found");

        if (job.AssignedDriverId != command.DriverId)
            throw new UnauthorizedAccessException("Only the assigned driver can start pickup");

        job.StartPickup();

        await _dispatchJobRepository.UpdateAsync(job, cancellationToken);
        await _dispatchJobRepository.SaveChangesAsync(cancellationToken);
    }
}