namespace ShoppingBasket.Core.Pricing;

/// <summary>
/// Strategy for turning "N of a product at unit price P" into a charge.
/// New offers are added by writing a new rule, without touching the calculator
/// (Open/Closed principle).
/// </summary>
public interface IPricingRule
{
    /// <summary>Short human-readable name of the offer, shown on the receipt.</summary>
    string Description { get; }

    /// <summary>Returns the amount to charge, in pence.</summary>
    int CalculatePence(int quantity, int unitPricePence);
}
