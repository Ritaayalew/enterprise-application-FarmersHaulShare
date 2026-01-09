using SharedKernel.Domain;
using FarmersHaulShare.SharedKernel.Domain;
using TransportMarketplaceAndDispatch.Domain.Entities;
using TransportMarketplaceAndDispatch.Domain.Events;
using TransportMarketplaceAndDispatch.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace TransportMarketplaceAndDispatch.Domain.Aggregates;

public sealed class Driver : AggregateRoot<Guid>
{

    private readonly List<Vehicle> _vehicles = new();
    public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

    public string UserId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public Location? CurrentLocation { get; private set; }
    public AvailabilityWindow? AvailabilityWindow { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private Driver() { } // EF Core

    [SetsRequiredMembers]
    public Driver(Guid id, string userId, string name, string phoneNumber)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        Id = id; // Explicitly set for required property
        UserId = userId;
        Name = name;
        PhoneNumber = phoneNumber;
        IsActive = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        if (_vehicles.Any(v => v.Id == vehicle.Id))
            throw new InvalidOperationException("Vehicle already added");

        _vehicles.Add(vehicle);
    }

    public void VerifyVehicle(Guid vehicleId)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (vehicle == null)
            throw new InvalidOperationException("Vehicle not found");

        vehicle.Verify();
    }

    public void UpdateAvailability(Location? currentLocation, AvailabilityWindow? availabilityWindow)
    {
        CurrentLocation = currentLocation;
        AvailabilityWindow = availabilityWindow;
        IsActive = availabilityWindow != null && currentLocation != null;

        RaiseDomainEvent(new DriverAvailabilityUpdated(Id));
    }
}