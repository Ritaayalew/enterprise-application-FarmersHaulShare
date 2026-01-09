namespace CatalogAndContracts.Domain.ValueObjects
{
    public class AgreementTerms
    {
        public int DeliveryDays { get; }
        public string QualityStandard { get; }

        public AgreementTerms(int deliveryDays, string qualityStandard)
        {
            DeliveryDays = deliveryDays;
            QualityStandard = qualityStandard;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not AgreementTerms other) return false;
            return DeliveryDays == other.DeliveryDays && QualityStandard == other.QualityStandard;
        }

        public override int GetHashCode() => HashCode.Combine(DeliveryDays, QualityStandard);
    }
}