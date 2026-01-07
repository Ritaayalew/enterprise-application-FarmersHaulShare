namespace SharedKernel.Domain;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
