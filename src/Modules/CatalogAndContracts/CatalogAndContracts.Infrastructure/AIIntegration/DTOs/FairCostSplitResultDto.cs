namespace CatalogAndContracts.Infrastructure.AIIntegration.DTOs
{
    public class FairCostSplitResultDto
    {
        public Guid RequestId { get; set; }
        public Dictionary<Guid, decimal> FarmerShares { get; set; } = new();
    }
}