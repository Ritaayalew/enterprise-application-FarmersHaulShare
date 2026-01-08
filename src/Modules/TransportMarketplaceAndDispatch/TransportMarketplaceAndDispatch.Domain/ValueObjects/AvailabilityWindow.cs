using SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.ValueObjects;

public sealed class AvailabilityWindow : ValueObject
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public DayOfWeek[] AvailableDays { get; }

    private AvailabilityWindow() { AvailableDays = Array.Empty<DayOfWeek>(); } // EF Core

    public AvailabilityWindow(DateTime startTime, DateTime endTime, DayOfWeek[]? availableDays = null)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        StartTime = startTime;
        EndTime = endTime;
        AvailableDays = availableDays ?? Enum.GetValues<DayOfWeek>();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
        foreach (var day in AvailableDays.OrderBy(d => d))
        {
            yield return day;
        }
    }

    public bool IsAvailableAt(DateTime dateTime)
    {
        if (dateTime < StartTime || dateTime > EndTime)
            return false;

        return AvailableDays.Contains(dateTime.DayOfWeek);
    }

    public bool OverlapsWith(AvailabilityWindow other)
    {
        return StartTime <= other.EndTime && EndTime >= other.StartTime;
    }
}