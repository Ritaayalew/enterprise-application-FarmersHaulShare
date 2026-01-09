using PricingAndFairCostSplit.Application.DTOs;
using PricingAndFairCostSplit.Application.Commands;
using PricingAndFairCostSplit.Domain.Aggregates;
using PricingAndFairCostSplit.Domain.ValueObjects;
using PricingAndFairCostSplit.Application.Interfaces;

namespace PricingAndFairCostSplit.Application.Services;

public class FairPricingAppService : IFairPricingAppService
{
    private readonly IFairCostSplitRepository _repository;

    public FairPricingAppService(IFairCostSplitRepository repository)
    {
        _repository = repository;
    }

    public async Task<FairCostSplitDto> CalculateFairCostSplit(CalculateFairCostSplitCommand command)
    {
        var totalTransport = new Money(command.TotalTransportCost);
        var pricePerKg = new PricePerKg(command.PricePerKg);

        var fairCostSplit = new FairCostSplit(command.HaulShareId, pricePerKg, command.TotalKg, totalTransport);

        // Calculate farmer shares
        foreach (var farmer in command.Farmers)
        {
            decimal percentage = (farmer.KgDelivered / command.TotalKg) * 100;
            var shareAmount = pricePerKg.AmountPerKg.Multiply(farmer.KgDelivered);

            fairCostSplit.AddFarmerShare(new FarmerShare(farmer.FarmerId, percentage, shareAmount));
        }

        await _repository.AddAsync(fairCostSplit);

        return new FairCostSplitDto(
            fairCostSplit.HaulShareId,
            fairCostSplit.TotalRevenue.Amount,
            fairCostSplit.TotalTransportCost.Amount,
            fairCostSplit.FarmerShares
                .Select(s => new CostShareDto(s.FarmerId, s.Percentage, s.ShareAmount.Amount))
                .ToList()
        );
    }
}
