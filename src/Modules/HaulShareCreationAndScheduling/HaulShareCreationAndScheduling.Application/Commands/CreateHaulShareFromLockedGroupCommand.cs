using HaulShareCreationAndScheduling.Application.DTOs;

namespace HaulShareCreationAndScheduling.Application.Commands;

public class CreateHaulShareFromLockedGroupCommand
{
    public Guid LockedGroupId { get; init; }
    public List<LockedBatchDto> Batches { get; init; } = [];
    public DateTime TargetPickupTime { get; init; }
    public DateTime DeliverBy { get; init; }
}
