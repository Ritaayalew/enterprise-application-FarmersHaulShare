namespace HaulShareCreationAndScheduling.Infrastructure.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public DateTime OccurredOnUtc { get; set; }
    public bool Published { get; set; }
}
