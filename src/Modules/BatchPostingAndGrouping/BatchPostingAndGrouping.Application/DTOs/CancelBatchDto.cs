namespace BatchPostingAndGrouping.Application.DTOs;

public sealed record CancelBatchDto
{
    public required string Reason { get; init; }
}
