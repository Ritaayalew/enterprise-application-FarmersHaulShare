namespace CatalogAndContracts.Infrastructure.AIIntegration.DTOs
{
    public class BatchAnalysisResultDto
    {
        public Guid BatchId { get; set; }
        public string QualityGrade { get; set; } = string.Empty;
        public double SuggestedPricePerKg { get; set; }
    }
}