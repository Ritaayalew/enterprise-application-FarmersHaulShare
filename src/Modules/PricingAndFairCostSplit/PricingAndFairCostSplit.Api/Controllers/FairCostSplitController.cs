using Microsoft.AspNetCore.Mvc;
using PricingAndFairCostSplit.Application.Services;
using PricingAndFairCostSplit.Application.Commands;
using PricingAndFairCostSplit.Domain.ValueObjects;

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
    public async Task<IActionResult> Calculate([FromBody] CalculateFairCostSplitRequest request)
    {
        var command = new CalculateFairCostSplitCommand(
            request.HaulShareId,
            new PricePerKg(new Money(request.PricePerKg)),
            request.TotalKg,
            request.TotalTransportCost
        );

        var result = await _service.CalculateFairCostSplit(command);
        return Ok(result);
    }
}

public record CalculateFairCostSplitRequest(
    Guid HaulShareId,
    decimal PricePerKg,
    decimal TotalKg,
    decimal TotalTransportCost
);
