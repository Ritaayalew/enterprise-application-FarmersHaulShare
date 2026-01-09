using PricingAndFairCostSplit.Domain.ValueObjects;
using FarmersHaulShare.SharedKernel.Domain;
using SharedKernel.Domain;
namespace PricingAndFairCostSplit.Domain.Services;

public interface IFairPricingService
{
    Money CalculateRevenue(PricePerKg pricePerKg, decimal totalKg);

    CostShare CalculateFarmerShare(
        Guid farmerId,
        decimal farmerKg,
        decimal totalKg,
        Money totalTransportCost
    );
}
