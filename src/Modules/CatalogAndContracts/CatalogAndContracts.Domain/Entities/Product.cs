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
    }
}