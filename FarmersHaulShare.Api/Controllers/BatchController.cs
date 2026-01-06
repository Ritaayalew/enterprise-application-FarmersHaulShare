using FarmersHaulShare.BatchPosting.Domain.Aggregates;
using FarmersHaulShare.BatchPosting.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FarmersHaulShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatchController : ControllerBase
{
    private readonly IBatchRepository _repository;

    public BatchController(IBatchRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int quantity = 100, DateTime? readyDate = null)
    {
        var batch = new Batch(quantity, readyDate ?? DateTime.UtcNow.AddDays(1));
        await _repository.AddAsync(batch);
        return CreatedAtAction(nameof(Create), new { id = batch.Id });
    }
}