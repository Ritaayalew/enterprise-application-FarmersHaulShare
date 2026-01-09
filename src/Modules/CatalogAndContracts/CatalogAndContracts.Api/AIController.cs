using Microsoft.AspNetCore.Mvc;
using BatchPostingAndGrouping.Application.DTOs;
using CatalogAndContracts.Infrastructure.AIIntegration.DTOs;
using CatalogAndContracts.Infrastructure.AIIntegration.Requests;
using CatalogAndContracts.Infrastructure.AIIntegration.Services;

namespace CatalogAndContracts.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly BatchAnalysisService _batchAnalysisService;
        private readonly FairCostSplitService _fairCostSplitService;
        private readonly DemandRoutingService _demandRoutingService;

        public AIController(
            BatchAnalysisService batchAnalysisService,
            FairCostSplitService fairCostSplitService,
            DemandRoutingService demandRoutingService)
        {
            _batchAnalysisService = batchAnalysisService;
            _fairCostSplitService = fairCostSplitService;
            _demandRoutingService = demandRoutingService;
        }

        [HttpPost("analyze-batch")]
        public ActionResult<BatchAnalysisResultDto> AnalyzeBatch([FromBody] BatchDto batch)
        {
            var result = _batchAnalysisService.Analyze(batch);
            return Ok(result);
        }

        [HttpPost("split-costs")]
        public ActionResult<FairCostSplitResultDto> SplitCosts([FromBody] FairCostSplitRequest request)
        {
            var result = _fairCostSplitService.SplitCosts(request);
            return Ok(result);
        }

        [HttpPost("suggest-route")]
        public ActionResult<DemandRoutingHintDto> SuggestRoute([FromBody] BatchDto batch)
        {
            var result = _demandRoutingService.SuggestRoute(batch);
            return Ok(result);
        }
    }
}