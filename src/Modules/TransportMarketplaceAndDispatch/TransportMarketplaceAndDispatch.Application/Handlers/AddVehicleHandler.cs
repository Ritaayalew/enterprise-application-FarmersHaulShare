using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Domain.Entities;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Domain.ValueObjects;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class AddVehicleHandler
{
    private readonly IDriverRepository _driverRepository;

    public AddVehicleHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<Guid> Handle(AddVehicleCommand command, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver == null)
            throw new InvalidOperationException($"Driver with ID {command.DriverId} not found");

        VehicleType vehicleType = command.VehicleTypeName.ToLower() switch
        {
            "truck" => VehicleType.Truck,
            "motorbike" => VehicleType.Motorbike,
            "van" => VehicleType.Van,
            _ => command.MaxWeightKg.HasValue && command.MaxVolumeCubicMeters.HasValue
                ? new VehicleType(command.VehicleTypeName, command.MaxWeightKg.Value, command.MaxVolumeCubicMeters.Value)
                : throw new ArgumentException($"Unknown vehicle type: {command.VehicleTypeName}")
        };

        var vehicle = new Vehicle(
            Guid.NewGuid(),
            command.DriverId,
            command.PlateNumber,
            vehicleType
        );

        driver.AddVehicle(vehicle);

        await _driverRepository.UpdateAsync(driver, cancellationToken);
        await _driverRepository.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}