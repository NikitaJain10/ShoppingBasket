namespace ShoppingBasket.Core.Pricing;

/// <summary>No offer: every item is charged at its unit price.</summary>
public sealed class UnitPricingRule : IPricingRule
{
    public static readonly UnitPricingRule Instance = new();

    private UnitPricingRule()
    {
    }

    public string Description => "No offer";

    public int CalculatePence(int quantity, int unitPricePence)
    {
        PricingGuard.Validate(quantity, unitPricePence);
        return checked(quantity * unitPricePence);
    }
}
