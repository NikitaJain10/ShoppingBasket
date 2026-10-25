using ShoppingBasket.Core.Models;

namespace ShoppingBasket.Core;

public interface IBasketCalculator
{
    /// <summary>Total cost of the basket in pence.</summary>
    int CalculateTotalPence(IEnumerable<string> items);

    /// <summary>Full breakdown of the basket: one line per product, plus totals.</summary>
    BasketResult Calculate(IEnumerable<string> items);
}
