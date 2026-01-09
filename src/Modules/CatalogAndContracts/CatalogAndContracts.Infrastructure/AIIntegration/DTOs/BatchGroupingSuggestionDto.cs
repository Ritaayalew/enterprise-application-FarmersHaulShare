namespace CatalogAndContracts.Infrastructure.AIIntegration.DTOs
{
    public class BatchGroupingSuggestionDto
    {
        public Guid GroupId { get; set; }
        public List<Guid> BatchIds { get; set; } = new();
        public string Reason { get; set; } = string.Empty;
    }
}