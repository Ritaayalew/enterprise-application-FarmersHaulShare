using BatchPostingAndGrouping.Infrastructure;
using FarmersHaulShare.SharedKernel;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Text.Json;

namespace FarmersHaulShare.Api;

public class OutboxPublisherJob : IJob
{
    private readonly BatchPostingAndGroupingDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OutboxPublisherJob> _logger;

    public OutboxPublisherJob(BatchPostingAndGroupingDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<OutboxPublisherJob> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var pending = await _dbContext.OutboxMessages
            .Where(o => o.Status == OutboxMessage.OutboxMessageStatus.Pending || 
                        (o.Status == OutboxMessage.OutboxMessageStatus.Failed && o.RetryCount < 5))
            .OrderBy(o => o.OccurredOn)
            .Take(20)
            .ToListAsync();

        foreach (var msg in pending)
        {
            try
            {
                var eventType = Type.GetType(msg.EventType);
                if (eventType == null) throw new Exception("Type not found");

                var domainEvent = JsonSerializer.Deserialize(msg.Payload, eventType);
                if (domainEvent == null) throw new Exception("Deserialization failed");

                await _publishEndpoint.Publish(domainEvent);

                msg.MarkAsPublished();
                _logger.LogInformation("Published event {Id}", msg.Id);
            }
            catch (Exception ex)
            {
                msg.MarkAsFailed();
                _logger.LogError(ex, "Failed to publish event {Id}, retry {Retry}", msg.Id, msg.RetryCount);
            }
        }

        if (pending.Any())
            await _dbContext.SaveChangesAsync();
    }
}