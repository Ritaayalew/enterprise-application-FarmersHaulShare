namespace CatalogAndContracts.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal BasePrice { get; private set; }

        public Product(string name, decimal basePrice)
        {
            Id = Guid.NewGuid();
            Name = name;
            BasePrice = basePrice;
        }
        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Product name cannot be empty.", nameof(newName));

            Name = newName;
        }

        public void UpdateBasePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(newPrice), "Base price must be greater than zero.");

            BasePrice = newPrice;
        }
    }
}
