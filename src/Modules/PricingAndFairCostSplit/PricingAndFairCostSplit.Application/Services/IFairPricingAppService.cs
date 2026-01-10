using PricingAndFairCostSplit.Application.DTOs;
using PricingAndFairCostSplit.Application.Commands;
using PricingAndFairCostSplit.Domain.Aggregates;
using PricingAndFairCostSplit.Domain.ValueObjects;


namespace PricingAndFairCostSplit.Application.Services;

public interface IFairPricingAppService
{
    Task<FairCostSplitDto> CalculateFairCostSplit(CalculateFairCostSplitCommand command);
}
