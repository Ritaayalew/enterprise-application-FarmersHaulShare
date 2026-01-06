using HaulShareCreationAndScheduling.Application.Commands;
using HaulShareCreationAndScheduling.Domain.Aggregates;
using HaulShareCreationAndScheduling.Domain.Entities;
using HaulShareCreationAndScheduling.Domain.Repositories;
using HaulShareCreationAndScheduling.Domain.ValueObjects;

namespace HaulShareCreationAndScheduling.Application.Handlers;

public class CreateHaulShareFromLockedGroupHandler
{
    private readonly IHaulShareRepository _repository;

    public CreateHaulShareFromLockedGroupHandler(IHaulShareRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateHaulShareFromLockedGroupCommand command,
        CancellationToken cancellationToken)
    {
        var pickupStops = command.Batches.Select(b =>
            new PickupStop(
                b.FarmerId,
                b.Latitude,
                b.Longitude,
                b.WeightKg
            )
        ).ToList();

        var totalWeight = pickupStops.Sum(p => p.WeightKg);

        var capacityPlan = new CapacityPlan(
            totalWeight,
            maxWeightKg: 3000
        );

        if (!capacityPlan.Fits())
            throw new InvalidOperationException("Vehicle capacity exceeded.");

        var haulShare = new HaulShare(
            Guid.NewGuid(),
            pickupStops,
            capacityPlan,
            new DeliveryWindow(command.TargetPickupTime, command.DeliverBy),
            command.TargetPickupTime
        );

        await _repository.AddAsync(haulShare, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return haulShare.Id;
    }
}
