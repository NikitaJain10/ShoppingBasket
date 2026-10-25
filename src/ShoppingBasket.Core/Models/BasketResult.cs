namespace ShoppingBasket.Core.Models;

/// <summary>The outcome of pricing a basket: every line plus the totals.</summary>
public sealed class BasketResult
{
    public BasketResult(IReadOnlyList<BasketLine> lines)
    {
        Lines = lines ?? throw new ArgumentNullException(nameof(lines));
    }

    public IReadOnlyList<BasketLine> Lines { get; }

    public int SubtotalPence => Lines.Sum(l => l.FullPricePence);

    public int TotalSavingsPence => Lines.Sum(l => l.SavingPence);

    public int TotalPence => Lines.Sum(l => l.ChargedPence);
}
