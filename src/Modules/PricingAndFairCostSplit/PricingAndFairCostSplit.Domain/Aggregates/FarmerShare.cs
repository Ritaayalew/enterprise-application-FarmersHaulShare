using PricingAndFairCostSplit.Domain.ValueObjects;

namespace PricingAndFairCostSplit.Domain.Aggregates;

public class FarmerShare
{
    public Guid FarmerId { get; private set; }
    public decimal Percentage { get; private set; } // 0-100%
    public Money ShareAmount { get; private set; }

    public FarmerShare(Guid farmerId, decimal percentage, Money shareAmount)
    {
        FarmerId = farmerId;
        Percentage = percentage;
        ShareAmount = shareAmount;
    }
}
