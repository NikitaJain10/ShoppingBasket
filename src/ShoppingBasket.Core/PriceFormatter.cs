using System.Globalization;

namespace ShoppingBasket.Core;

/// <summary>Formats pence for display: 85 → "85p", 135 → "£1.35".</summary>
public static class PriceFormatter
{
    public static string Format(int pence)
    {
        if (pence < 0)
            return "-" + Format(-pence);

        return pence < 100
            ? $"{pence}p"
            : "£" + (pence / 100m).ToString("0.00", CultureInfo.InvariantCulture);
    }
}
