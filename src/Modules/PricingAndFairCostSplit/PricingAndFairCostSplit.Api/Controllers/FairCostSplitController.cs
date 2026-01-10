using Microsoft.AspNetCore.Mvc;
using PricingAndFairCostSplit.Application.Services;
using PricingAndFairCostSplit.Application.Commands;
using PricingAndFairCostSplit.Application.DTOs;

namespace PricingAndFairCostSplit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FairCostSplitController : ControllerBase
{
    private readonly IFairPricingAppService _service;

    public FairCostSplitController(IFairPricingAppService service)
    {
        _service = service;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<FairCostSplitDto>> Calculate([FromBody] CalculateFairCostSplitCommand command)
    {
        var result = await _service.CalculateFairCostSplit(command);
        return Ok(result);
    }
}
