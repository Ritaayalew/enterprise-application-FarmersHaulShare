namespace PricingAndFairCostSplit.Application.DTOs;

public record CostShareDto(Guid FarmerId, decimal Percentage, decimal ShareAmount);

public record FairCostSplitDto(
    Guid HaulShareId,
    decimal TotalRevenue,
    decimal TotalTransportCost,
    List<CostShareDto> FarmerShares
);
