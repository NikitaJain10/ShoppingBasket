using ShoppingBasket.Core.Catalogue;
using ShoppingBasket.Core.Exceptions;
using ShoppingBasket.Core.Models;

namespace ShoppingBasket.Core;

/// <summary>
/// Prices a basket given as a flat list of item names, e.g. ["Apple", "Apple", "Banana"].
/// Steps: validate every name → count items per product → apply each product's pricing rule.
/// Offers are counted across the whole basket, so the order of items doesn't matter.
/// </summary>
public sealed class BasketCalculator : IBasketCalculator
{
    private readonly IProductCatalogue _catalogue;

    public BasketCalculator(IProductCatalogue catalogue)
    {
        _catalogue = catalogue ?? throw new ArgumentNullException(nameof(catalogue));
    }

    public int CalculateTotalPence(IEnumerable<string> items) => Calculate(items).TotalPence;

    public BasketResult Calculate(IEnumerable<string> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        // Keep first-seen order so the receipt reads in the order items were scanned.
        var counts = new Dictionary<string, (Product Product, int Quantity)>(StringComparer.OrdinalIgnoreCase);
        var order = new List<string>();

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentException("Basket contains an empty item name.", nameof(items));

            if (!_catalogue.TryGetProduct(item, out var product))
                throw new UnknownItemException(item.Trim());

            if (counts.TryGetValue(product.Name, out var entry))
            {
                counts[product.Name] = (product, entry.Quantity + 1);
            }
            else
            {
                counts[product.Name] = (product, 1);
                order.Add(product.Name);
            }
        }

        var lines = order
            .Select(name => counts[name])
            .Select(e => new BasketLine(
                ProductName: e.Product.Name,
                Quantity: e.Quantity,
                UnitPricePence: e.Product.UnitPricePence,
                FullPricePence: checked(e.Quantity * e.Product.UnitPricePence),
                ChargedPence: e.Product.PricingRule.CalculatePence(e.Quantity, e.Product.UnitPricePence),
                OfferDescription: e.Product.PricingRule.Description))
            .ToList();

        return new BasketResult(lines);
    }
}
