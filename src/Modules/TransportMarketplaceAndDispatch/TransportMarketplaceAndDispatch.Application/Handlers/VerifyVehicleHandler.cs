using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class VerifyVehicleHandler
{
    private readonly IDriverRepository _driverRepository;

    public VerifyVehicleHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task Handle(VerifyVehicleCommand command, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver == null)
            throw new InvalidOperationException($"Driver with ID {command.DriverId} not found");

        driver.VerifyVehicle(command.VehicleId);

        await _driverRepository.UpdateAsync(driver, cancellationToken);
        await _driverRepository.SaveChangesAsync(cancellationToken);
    }
}