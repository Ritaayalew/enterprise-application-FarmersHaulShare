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

        // EF Core requires a parameterless constructor
        private Catalog() { }

        // Explicit constructor with initialization
        public Catalog(Guid id)
        {
            Id = id;
        }

        public void AddProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (_products.Exists(p => p.Name == product.Name))
                throw new InvalidOperationException("Product with the same name already exists in the catalog.");

            _products.Add(product);
        }

        public void RemoveProduct(Guid productId)
        {
            var product = _products.Find(p => p.Id == productId);
            if (product == null) throw new KeyNotFoundException("Product not found in catalog.");

            _products.Remove(product);
        }

        public Product? FindProductById(Guid productId)
        {
            return _products.Find(p => p.Id == productId);
        }
    }
}