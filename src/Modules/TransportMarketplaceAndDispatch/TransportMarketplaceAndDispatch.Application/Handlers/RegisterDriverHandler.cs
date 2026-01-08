using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Domain.Aggregates;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class RegisterDriverHandler
{
    private readonly IDriverRepository _driverRepository;

    public RegisterDriverHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<Guid> Handle(RegisterDriverCommand command, CancellationToken cancellationToken)
    {
        // Check if driver already exists
        var existingDriver = await _driverRepository.GetByUserIdAsync(command.UserId, cancellationToken);
        if (existingDriver != null)
            throw new InvalidOperationException($"Driver with UserId {command.UserId} already exists");

        var driver = new Driver(
            Guid.NewGuid(),
            command.UserId,
            command.Name,
            command.PhoneNumber
        );

        await _driverRepository.AddAsync(driver, cancellationToken);
        await _driverRepository.SaveChangesAsync(cancellationToken);

        return driver.Id;
    }
}