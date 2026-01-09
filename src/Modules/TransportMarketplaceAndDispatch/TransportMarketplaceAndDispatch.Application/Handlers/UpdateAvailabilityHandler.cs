using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Domain.ValueObjects;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class UpdateAvailabilityHandler
{
    private readonly IDriverRepository _driverRepository;

    public UpdateAvailabilityHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task Handle(UpdateAvailabilityCommand command, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver == null)
            throw new InvalidOperationException($"Driver with ID {command.DriverId} not found");

        Location? location = null;
        if (command.CurrentLatitude.HasValue && command.CurrentLongitude.HasValue)
        {
            location = new Location(command.CurrentLatitude.Value, command.CurrentLongitude.Value);
        }

        AvailabilityWindow? window = null;
        if (command.AvailabilityStartTime.HasValue && command.AvailabilityEndTime.HasValue)
        {
            window = new AvailabilityWindow(
                command.AvailabilityStartTime.Value,
                command.AvailabilityEndTime.Value,
                command.AvailableDays
            );
        }

        driver.UpdateAvailability(location, window);

        await _driverRepository.UpdateAsync(driver, cancellationToken);
        await _driverRepository.SaveChangesAsync(cancellationToken);
    }
}