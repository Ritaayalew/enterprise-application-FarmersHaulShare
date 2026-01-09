using System;
using System.Collections.Generic;
using CatalogAndContracts.Domain.Entities;
using CatalogAndContracts.Domain.ValueObjects;
using CatalogAndContracts.Domain.Events;

namespace CatalogAndContracts.Domain.Aggregates
{
    public class Contract
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }

        public decimal Price { get; private set; }
        public string BuyerId { get; private set; } = string.Empty;
        public string FarmerId { get; private set; } = string.Empty;
        public AgreementTerms? Terms { get; private set; }

        private readonly List<object> _events = new();
        public IReadOnlyCollection<object> Events => _events.AsReadOnly();

        // EF Core requires a parameterless constructor
        private Contract() { }

        public Contract(Product product, decimal price, string buyerId, string farmerId)
        {
            Id = Guid.NewGuid();
            Product = product ?? throw new ArgumentNullException(nameof(product));
            ProductId = product.Id;
            Price = price;
            BuyerId = buyerId ?? throw new ArgumentNullException(nameof(buyerId));
            FarmerId = farmerId ?? throw new ArgumentNullException(nameof(farmerId));

            AddEvent(new ContractCreated(Id, Product.Name, BuyerId, FarmerId));
        }

        public void SetAgreementTerms(AgreementTerms terms)
        {
            Terms = terms ?? throw new ArgumentNullException(nameof(terms));
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0) throw new ArgumentOutOfRangeException(nameof(newPrice), "Price must be positive.");
            Price = newPrice;
        }

        private void AddEvent(object @event) => _events.Add(@event);
    }
}