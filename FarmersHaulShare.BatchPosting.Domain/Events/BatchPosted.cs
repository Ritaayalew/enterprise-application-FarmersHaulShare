using FarmersHaulShare.SharedKernel;

namespace FarmersHaulShare.BatchPosting.Domain.Events;

public record BatchPosted(Guid BatchId, int Quantity, DateTime ReadyDate) : IDomainEvent;