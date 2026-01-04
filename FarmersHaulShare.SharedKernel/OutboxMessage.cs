using System;

namespace FarmersHaulShare.SharedKernel;

public class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string EventType { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

    public DateTime? ProcessedOn { get; private set; }

    public OutboxMessageStatus Status { get; private set; } = OutboxMessageStatus.Pending;

    public int RetryCount { get; private set; } = 0;

    public enum OutboxMessageStatus
    {
        Pending,
        Published,
        Failed
    }

    // EF Core needs parameterless constructor
    private OutboxMessage() { }

    public OutboxMessage(string eventType, object @event)
    {
        EventType = eventType;
        Payload = System.Text.Json.JsonSerializer.Serialize(@event);
    }

    public void MarkAsPublished()
    {
        ProcessedOn = DateTime.UtcNow;
        Status = OutboxMessageStatus.Published;
    }

    public void MarkAsFailed()
    {
        RetryCount++;
        Status = OutboxMessageStatus.Failed;
    }
}