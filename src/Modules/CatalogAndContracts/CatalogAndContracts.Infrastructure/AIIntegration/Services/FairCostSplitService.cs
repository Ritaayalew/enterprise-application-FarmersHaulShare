using CatalogAndContracts.Infrastructure.AIIntegration.DTOs;
using CatalogAndContracts.Infrastructure.AIIntegration.Requests;

namespace CatalogAndContracts.Infrastructure.AIIntegration.Services
{
    public class FairCostSplitService
    {
        public FairCostSplitResultDto SplitCosts(FairCostSplitRequest request)
        {
            var sharePerFarmer = request.TotalCost / request.FarmerIds.Count;
            var result = new FairCostSplitResultDto
            {
                RequestId = request.RequestId
            };

            foreach (var farmerId in request.FarmerIds)
            {
                result.FarmerShares[farmerId] = sharePerFarmer;
            }

            return result;
        }
    }
}