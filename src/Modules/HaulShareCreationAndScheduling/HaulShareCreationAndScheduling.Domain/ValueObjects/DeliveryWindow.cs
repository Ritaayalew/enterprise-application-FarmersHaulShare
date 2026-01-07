 namespace HaulShareCreationAndScheduling.Domain.ValueObjects;

public sealed class DeliveryWindow
{
    public DateTime Earliest { get; private set; }
    public DateTime Latest { get; private set; }

    private DeliveryWindow() { } // ✅ REQUIRED by EF Core

    public DeliveryWindow(DateTime earliest, DateTime latest)
    {
        if (latest <= earliest)
            throw new InvalidOperationException("Invalid delivery window.");

        Earliest = earliest;
        Latest = latest;
    }
}
