using FarmersHaulShare.SharedKernel;
using FarmersHaulShare.SharedKernel.Domain; // For IDomainEvent
using PricingAndFairCostSplit.Domain.ValueObjects;
using PricingAndFairCostSplit.Domain.Events;

namespace PricingAndFairCostSplit.Domain.Aggregates;

public class FairCostSplit : IHaveDomainEvents
{
    public Guid HaulShareId { get; private set; }
    public Money TotalRevenue { get; private set; }
    public Money TotalTransportCost { get; private set; }
    public List<CostShare> FarmerShares { get; private set; } = new();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    private FairCostSplit() { } // For EF

    public FairCostSplit(Guid haulShareId, PricePerKg pricePerKg, decimal totalKg, Money transportCost)
    {
        HaulShareId = haulShareId;
        TotalRevenue = pricePerKg.AmountPerKg.Multiply(totalKg);
        TotalTransportCost = transportCost;

        // Simple proportional split (can be more complex later)
        // Assume farmer quantities known — for demo, split equally or by quantity
        // Example: add shares here

        _domainEvents.Add(new PriceCalculated(HaulShareId, TotalRevenue, pricePerKg));
        _domainEvents.Add(new FairCostSplitDetermined(HaulShareId, FarmerShares, TotalTransportCost));
    }

    // Add method to add farmer shares, recalculate, raise events
    public void AddFarmerShare(Guid farmerId, decimal quantityKg, decimal totalKg)
    {
        var percentage = quantityKg / totalKg;
        var share = TotalTransportCost.Multiply(percentage);
        FarmerShares.Add(new CostShare(farmerId, share, percentage * 100));

        // Raise updated event if needed
    }
}