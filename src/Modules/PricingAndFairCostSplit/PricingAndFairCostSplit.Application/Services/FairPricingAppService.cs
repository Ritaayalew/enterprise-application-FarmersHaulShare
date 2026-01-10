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
        var fairCostSplit = new FairCostSplit(command.HaulShareId, command.PricePerKg, command.TotalKg, totalTransport);

        // Here we could add farmer shares if needed, for demo it can stay empty

        await _repository.AddAsync(fairCostSplit);

        return new FairCostSplitDto(
            fairCostSplit.HaulShareId,
            fairCostSplit.TotalRevenue.Amount,
            fairCostSplit.TotalTransportCost.Amount,
            fairCostSplit.FarmerShares.Select(s => new CostShareDto(s.FarmerId, s.Percentage, s.ShareAmount.Amount)).ToList()
        );
    }
}
