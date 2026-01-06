namespace HaulShareCreationAndScheduling.Domain.ValueObjects;

public sealed class DeliveryWindow
{
    public DateTime Earliest { get; }
    public DateTime Latest { get; }

    public DeliveryWindow(DateTime earliest, DateTime latest)
    {
        if (latest <= earliest)
            throw new InvalidOperationException("Invalid delivery window.");

        Earliest = earliest;
        Latest = latest;
    }
}
