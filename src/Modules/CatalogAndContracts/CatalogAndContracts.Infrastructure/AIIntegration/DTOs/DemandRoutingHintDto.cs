namespace CatalogAndContracts.Infrastructure.AIIntegration.DTOs
{
    public class DemandRoutingHintDto
    {
        public Guid BatchId { get; set; }
        public string SuggestedRoute { get; set; } = string.Empty;
        public DateTime EstimatedDeliveryUtc { get; set; }
    }
}