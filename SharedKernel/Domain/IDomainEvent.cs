namespace FarmersHaulShare.SharedKernel.Domain
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
