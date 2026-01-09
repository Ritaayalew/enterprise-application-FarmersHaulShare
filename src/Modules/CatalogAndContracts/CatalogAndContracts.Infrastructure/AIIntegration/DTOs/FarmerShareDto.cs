namespace CatalogAndContracts.Infrastructure.AIIntegration.DTOs
{
    public class FarmerShareDto
    {
        public Guid FarmerId { get; set; }
        public decimal ShareAmount { get; set; }
    }
}