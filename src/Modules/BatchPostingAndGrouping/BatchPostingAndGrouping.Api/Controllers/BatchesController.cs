using BatchPostingAndGrouping.Application.DTOs;
using BatchPostingAndGrouping.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatchPostingAndGrouping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Farmer")]
public sealed class BatchesController : ControllerBase
{
    private readonly IBatchService _batchService;
    private readonly ILogger<BatchesController> _logger;

    public BatchesController(
        IBatchService batchService,
        ILogger<BatchesController> logger)
    {
        _batchService = batchService;
        _logger = logger;
    }

    /// <summary>
    /// Post a new batch
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BatchDto>> PostBatch(
        [FromBody] PostBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get farmer profile ID from claims (assuming it's in the token)
            // For now, we'll use a claim or header - adjust based on your identity setup
            var farmerProfileIdClaim = User.FindFirst("farmer_profile_id")?.Value
                ?? User.FindFirst("sub")?.Value // Fallback to subject claim
                ?? throw new UnauthorizedAccessException("Farmer profile ID not found in token.");

            if (!Guid.TryParse(farmerProfileIdClaim, out var farmerProfileId))
                throw new UnauthorizedAccessException("Invalid farmer profile ID format.");

            var batch = await _batchService.PostBatchAsync(farmerProfileId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetBatchById), new { id = batch.Id }, batch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error posting batch");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get batch by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BatchDto>> GetBatchById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var batch = await _batchService.GetBatchByIdAsync(id, cancellationToken);
        if (batch == null)
            return NotFound();

        return Ok(batch);
    }

    /// <summary>
    /// Get all batches for the current farmer
    /// </summary>
    [HttpGet("my-batches")]
    public async Task<ActionResult<IReadOnlyList<BatchDto>>> GetMyBatches(
        CancellationToken cancellationToken = default)
    {
        var farmerProfileIdClaim = User.FindFirst("farmer_profile_id")?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException("Farmer profile ID not found in token.");

        if (!Guid.TryParse(farmerProfileIdClaim, out var farmerProfileId))
            throw new UnauthorizedAccessException("Invalid farmer profile ID format.");

        var batches = await _batchService.GetBatchesByFarmerAsync(farmerProfileId, cancellationToken);
        return Ok(batches);
    }

    /// <summary>
    /// Update a batch
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BatchDto>> UpdateBatch(
        Guid id,
        [FromBody] UpdateBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var farmerProfileIdClaim = User.FindFirst("farmer_profile_id")?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("Farmer profile ID not found in token.");

            if (!Guid.TryParse(farmerProfileIdClaim, out var farmerProfileId))
                throw new UnauthorizedAccessException("Invalid farmer profile ID format.");

            var batch = await _batchService.UpdateBatchAsync(id, farmerProfileId, dto, cancellationToken);
            return Ok(batch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating batch {BatchId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a batch
    /// </summary>
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelBatch(
        Guid id,
        [FromBody] CancelBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var farmerProfileIdClaim = User.FindFirst("farmer_profile_id")?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("Farmer profile ID not found in token.");

            if (!Guid.TryParse(farmerProfileIdClaim, out var farmerProfileId))
                throw new UnauthorizedAccessException("Invalid farmer profile ID format.");

            await _batchService.CancelBatchAsync(id, farmerProfileId, dto, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling batch {BatchId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
