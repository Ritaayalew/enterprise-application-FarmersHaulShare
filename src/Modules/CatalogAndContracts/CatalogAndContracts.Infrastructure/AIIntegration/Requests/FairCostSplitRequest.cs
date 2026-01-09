namespace CatalogAndContracts.Infrastructure.AIIntegration.Requests
{
    public class FairCostSplitRequest
    {
        public Guid RequestId { get; set; }
        public List<Guid> FarmerIds { get; set; } = new();
        public decimal TotalCost { get; set; }
    }
}