using BatchPostingAndGrouping.Application.DTOs;
using BatchPostingAndGrouping.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatchPostingAndGrouping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class GroupCandidatesController : ControllerBase
{
    private readonly IGroupingService _groupingService;
    private readonly ILogger<GroupCandidatesController> _logger;

    public GroupCandidatesController(
        IGroupingService groupingService,
        ILogger<GroupCandidatesController> logger)
    {
        _groupingService = groupingService;
        _logger = logger;
    }

    /// <summary>
    /// Get all forming group candidates
    /// </summary>
    [HttpGet("forming")]
    public async Task<ActionResult<IReadOnlyList<GroupCandidateDto>>> GetFormingCandidates(
        [FromQuery] string? produceTypeName = null,
        CancellationToken cancellationToken = default)
    {
        var candidates = await _groupingService.GetFormingGroupCandidatesAsync(produceTypeName, cancellationToken);
        return Ok(candidates);
    }

    /// <summary>
    /// Get group candidate by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupCandidateDto>> GetGroupCandidateById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var candidate = await _groupingService.GetGroupCandidateByIdAsync(id, cancellationToken);
        if (candidate == null)
            return NotFound();

        return Ok(candidate);
    }

    /// <summary>
    /// Form a new group candidate from batch IDs
    /// </summary>
    [HttpPost("form")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<GroupCandidateDto>> FormGroupCandidate(
        [FromBody] FormGroupCandidateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var candidate = await _groupingService.FormGroupCandidateAsync(
                request.BatchIds,
                request.MaxDistanceKm ?? 50.0,
                request.MaxTimeWindowHours,
                cancellationToken);

            return CreatedAtAction(nameof(GetGroupCandidateById), new { id = candidate.Id }, candidate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error forming group candidate");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Lock a group candidate
    /// </summary>
    [HttpPost("{id}/lock")]
    [Authorize(Policy = "Coordinator")]
    public async Task<IActionResult> LockGroupCandidate(
        Guid id,
        [FromBody] LockGroupCandidateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _groupingService.LockGroupCandidateAsync(
                id,
                request.LockWindowStartUtc,
                request.LockWindowEndUtc,
                cancellationToken);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking group candidate {GroupCandidateId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}

public sealed record FormGroupCandidateRequest
{
    public required List<Guid> BatchIds { get; init; }
    public double? MaxDistanceKm { get; init; }
    public TimeSpan? MaxTimeWindowHours { get; init; }
}

public sealed record LockGroupCandidateRequest
{
    public required DateTime LockWindowStartUtc { get; init; }
    public required DateTime LockWindowEndUtc { get; init; }
}
