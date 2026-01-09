using PricingAndFairCostSplit.Domain.ValueObjects;
using PricingAndFairCostSplit.Domain.Services; // existing namespace for IFairPricingService
using SharedKernel.Domain;
namespace PricingAndFairCostSplit.Domain.Services
{
    public class FairPricingService : IFairPricingService
    {
        public Money CalculateRevenue(PricePerKg pricePerKg, decimal totalKg)
            => pricePerKg.AmountPerKg.Multiply(totalKg);

        public CostShare CalculateFarmerShare(Guid farmerId, decimal quantityKg, decimal totalKg, Money totalTransportCost)
        {
            var percentage = quantityKg / totalKg;
            var shareAmount = totalTransportCost.Multiply(percentage);
            return new CostShare(farmerId, shareAmount, percentage * 100);
        }
    }
}
