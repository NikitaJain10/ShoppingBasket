namespace ShoppingBasket.Core.Pricing;

/// <summary>
/// "Buy X, pay for Y" offers. One rule covers both offers in the brief:
/// <list type="bullet">
///   <item>Buy one get one free  = buy 2, pay for 1</item>
///   <item>Three for the price of two = buy 3, pay for 2</item>
/// </list>
/// Items that don't make up a full group are charged at the unit price.
/// </summary>
public sealed class MultiBuyPricingRule : IPricingRule
{
    public MultiBuyPricingRule(int groupSize, int chargedItemsPerGroup, string description)
    {
        if (groupSize < 1)
            throw new ArgumentOutOfRangeException(nameof(groupSize), "Group size must be at least 1.");
        if (chargedItemsPerGroup < 0 || chargedItemsPerGroup > groupSize)
            throw new ArgumentOutOfRangeException(nameof(chargedItemsPerGroup),
                "Charged items must be between 0 and the group size.");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description must not be empty.", nameof(description));

        GroupSize = groupSize;
        ChargedItemsPerGroup = chargedItemsPerGroup;
        Description = description;
    }

    public static MultiBuyPricingRule BuyOneGetOneFree() => new(2, 1, "Buy one get one free");

    public static MultiBuyPricingRule ThreeForTwo() => new(3, 2, "3 for the price of 2");

    public int GroupSize { get; }

    public int ChargedItemsPerGroup { get; }

    public string Description { get; }

    public int CalculatePence(int quantity, int unitPricePence)
    {
        PricingGuard.Validate(quantity, unitPricePence);

        var fullGroups = quantity / GroupSize;
        var remainder = quantity % GroupSize;
        var chargeableItems = checked(fullGroups * ChargedItemsPerGroup + remainder);

        return checked(chargeableItems * unitPricePence);
    }
}
