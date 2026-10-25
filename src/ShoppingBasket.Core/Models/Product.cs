using ShoppingBasket.Core.Pricing;

namespace ShoppingBasket.Core.Models;

/// <summary>
/// A product that can be bought. Prices are held in whole pence (int) so that
/// no floating-point rounding can ever creep into a total.
/// </summary>
public sealed record Product
{
    public Product(string name, int unitPricePence, IPricingRule pricingRule)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name must not be empty.", nameof(name));
        if (unitPricePence < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPricePence), "Unit price cannot be negative.");

        Name = name.Trim();
        UnitPricePence = unitPricePence;
        PricingRule = pricingRule ?? throw new ArgumentNullException(nameof(pricingRule));
    }

    public string Name { get; }

    public int UnitPricePence { get; }

    public IPricingRule PricingRule { get; }
}
