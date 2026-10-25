namespace ShoppingBasket.Core.Models;

/// <summary>One line of the receipt: a product, how many were bought and what they cost.</summary>
public sealed record BasketLine(
    string ProductName,
    int Quantity,
    int UnitPricePence,
    int FullPricePence,
    int ChargedPence,
    string OfferDescription)
{
    /// <summary>How much the offer saved on this line.</summary>
    public int SavingPence => FullPricePence - ChargedPence;
}
