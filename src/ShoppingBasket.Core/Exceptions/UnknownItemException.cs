namespace ShoppingBasket.Core.Exceptions;

/// <summary>Thrown when the basket contains an item the shop doesn't sell.</summary>
public sealed class UnknownItemException : Exception
{
    public UnknownItemException(string itemName)
        : base($"Unknown item '{itemName}'. It is not in the product catalogue.")
    {
        ItemName = itemName;
    }

    public string ItemName { get; }
}
