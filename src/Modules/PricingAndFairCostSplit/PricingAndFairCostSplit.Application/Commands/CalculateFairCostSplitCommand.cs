namespace PricingAndFairCostSplit.Application.Commands;

public class CalculateFairCostSplitCommand
{
    public Guid HaulShareId { get; set; }
    public decimal PricePerKg { get; set; }
    public decimal TotalKg { get; set; }
    public decimal TotalTransportCost { get; set; }

    public List<FarmerInputDto> Farmers { get; set; } = new List<FarmerInputDto>();
}

public class FarmerInputDto
{
    public Guid FarmerId { get; set; }
    public decimal KgDelivered { get; set; }
}
