using PricingAndFairCostSplit.Domain.ValueObjects;

namespace PricingAndFairCostSplit.Domain.Aggregates;

public class FairCostSplit
{
    public Guid HaulShareId { get; private set; }
    public Money TotalRevenue { get; private set; }
    public Money TotalTransportCost { get; private set; }

    private readonly List<FarmerShare> _farmerShares = new();
    public IReadOnlyList<FarmerShare> FarmerShares => _farmerShares.AsReadOnly();

    private FairCostSplit() { } // For EF

    public FairCostSplit(Guid haulShareId, PricePerKg pricePerKg, decimal totalKg, Money transportCost)
    {
        HaulShareId = haulShareId;
        TotalRevenue = pricePerKg.AmountPerKg.Multiply(totalKg);
        TotalTransportCost = transportCost;
    }

    public void AddFarmerShare(FarmerShare share)
    {
        _farmerShares.Add(share);
    }
}
