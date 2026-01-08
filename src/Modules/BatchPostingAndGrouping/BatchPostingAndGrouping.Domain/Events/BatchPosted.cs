using FarmersHaulShare.SharedKernel.Domain;

namespace FarmersHaulShare.BatchPostingAndGrouping.Domain.Events
{
    public class BatchPosted : IDomainEvent
    {
        public Guid BatchId { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public BatchPosted(Guid batchId)
        {
            BatchId = batchId;
        }
    }
}
