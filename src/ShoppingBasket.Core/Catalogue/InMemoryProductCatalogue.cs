using System.Diagnostics.CodeAnalysis;
using ShoppingBasket.Core.Models;
using ShoppingBasket.Core.Pricing;

namespace ShoppingBasket.Core.Catalogue;

/// <summary>Catalogue held in memory. Look-ups ignore case and surrounding spaces.</summary>
public sealed class InMemoryProductCatalogue : IProductCatalogue
{
    private readonly Dictionary<string, Product> _products;

    public InMemoryProductCatalogue(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        _products = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
        foreach (var product in products)
        {
            ArgumentNullException.ThrowIfNull(product, nameof(products));
            if (!_products.TryAdd(product.Name, product))
                throw new ArgumentException($"Duplicate product '{product.Name}' in catalogue.", nameof(products));
        }
    }

    /// <summary>The catalogue from the brief: apples, bananas, melons and limes.</summary>
    public static InMemoryProductCatalogue CreateDefault() => new(new[]
    {
        new Product("Apple", 35, UnitPricingRule.Instance),
        new Product("Banana", 20, UnitPricingRule.Instance),
        new Product("Melon", 50, MultiBuyPricingRule.BuyOneGetOneFree()),
        new Product("Lime", 15, MultiBuyPricingRule.ThreeForTwo()),
    });

    public bool TryGetProduct(string name, [NotNullWhen(true)] out Product? product)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            product = null;
            return false;
        }

        return _products.TryGetValue(name.Trim(), out product);
    }
}
