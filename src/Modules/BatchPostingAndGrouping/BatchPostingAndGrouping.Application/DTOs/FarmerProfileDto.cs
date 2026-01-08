namespace BatchPostingAndGrouping.Application.DTOs;

public sealed record FarmerProfileDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public double? DefaultLatitude { get; init; }
    public double? DefaultLongitude { get; init; }
    public string? DefaultAddress { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
