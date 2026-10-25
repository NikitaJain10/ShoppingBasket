using System.Diagnostics.CodeAnalysis;
using ShoppingBasket.Core.Models;

namespace ShoppingBasket.Core.Catalogue;

/// <summary>
/// Where products and their prices come from. Today it's in memory; in a real
/// system it could be a database or a pricing API, with no change to the calculator.
/// </summary>
public interface IProductCatalogue
{
    bool TryGetProduct(string name, [NotNullWhen(true)] out Product? product);
}
