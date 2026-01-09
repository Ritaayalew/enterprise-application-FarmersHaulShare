namespace CatalogAndContracts.Application.DTOs
{
    public class ContractDto
    {
        public Guid Id { get; set; }
        
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public string BuyerId { get; set; } = string.Empty;
        public string FarmerId { get; set; } = string.Empty;
    }
}