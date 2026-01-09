using System;
using System.Collections.Generic;
using CatalogAndContracts.Domain.ValueObjects;
using CatalogAndContracts.Domain.Events;

namespace CatalogAndContracts.Domain.Aggregates
{
    public class Contract
    {
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }

        public decimal Price { get; private set; }
        public string BuyerId { get; private set; } = string.Empty;
        public string FarmerId { get; private set; } = string.Empty;
        public AgreementTerms? Terms { get; private set; }

        private readonly List<object> _events = new();
        public IReadOnlyCollection<object> Events => _events.AsReadOnly();

        private Contract() { } // EF

        public Contract(Guid productId, decimal price, string buyerId, string farmerId)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Price = price;
            BuyerId = buyerId;
            FarmerId = farmerId;

            AddEvent(new ContractCreated(Id, ProductId, BuyerId, FarmerId));
        }

        public void SetAgreementTerms(AgreementTerms terms)
        {
            Terms = terms ?? throw new ArgumentNullException(nameof(terms));
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(newPrice));

            Price = newPrice;
        }

        private void AddEvent(object @event) => _events.Add(@event);
    }
}
