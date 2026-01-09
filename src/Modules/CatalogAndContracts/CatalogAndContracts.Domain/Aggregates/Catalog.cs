using System;
using System.Collections.Generic;
using CatalogAndContracts.Domain.Entities;

namespace CatalogAndContracts.Domain.Aggregates
{
    public class Catalog
    {
        public Guid Id { get; private set; }

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        // Required by EF Core
        private Catalog() { }

        // Public constructor for domain usage
        public Catalog(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Catalog ID cannot be empty.", nameof(id));

            Id = id;
        }

        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_products.Exists(p => p.Id == product.Id))
                throw new InvalidOperationException("Product already exists in catalog.");

            _products.Add(product);
        }

        public void RemoveProduct(Guid productId)
        {
            var product = _products.Find(p => p.Id == productId);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            _products.Remove(product);
        }

        public Product? FindProductById(Guid productId)
        {
            return _products.Find(p => p.Id == productId);
        }
    }
}
