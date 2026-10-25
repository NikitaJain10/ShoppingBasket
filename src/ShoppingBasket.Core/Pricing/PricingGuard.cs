namespace ShoppingBasket.Core.Pricing;

internal static class PricingGuard
{
    public static void Validate(int quantity, int unitPricePence)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
        if (unitPricePence < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPricePence), "Unit price cannot be negative.");
    }
}
