using ShoppingBasket.Core;
using ShoppingBasket.Core.Catalogue;
using ShoppingBasket.Core.Exceptions;

// Usage:
//   dotnet run --project src/ShoppingBasket.App -- Apple Apple Banana Melon Melon Lime Lime Lime
//   dotnet run --project src/ShoppingBasket.App            (then type items, comma-separated)

var items = args.Length > 0 ? args : ReadItemsFromConsole();

var calculator = new BasketCalculator(InMemoryProductCatalogue.CreateDefault());

try
{
    var result = calculator.Calculate(items);

    if (result.Lines.Count == 0)
    {
        Console.WriteLine("Basket is empty. Total: 0p");
        return 0;
    }

    Console.WriteLine();
    Console.WriteLine($"{"Item",-10}{"Qty",5}{"Unit",8}{"Price",9}{"Saving",9}  Offer");
    Console.WriteLine(new string('-', 64));
    foreach (var line in result.Lines)
    {
        Console.WriteLine(
            $"{line.ProductName,-10}{line.Quantity,5}{PriceFormatter.Format(line.UnitPricePence),8}" +
            $"{PriceFormatter.Format(line.ChargedPence),9}" +
            $"{(line.SavingPence > 0 ? "-" + PriceFormatter.Format(line.SavingPence) : ""),9}  {line.OfferDescription}");
    }

    Console.WriteLine(new string('-', 64));
    Console.WriteLine($"Subtotal: {PriceFormatter.Format(result.SubtotalPence)}");
    Console.WriteLine($"Savings:  {PriceFormatter.Format(result.TotalSavingsPence)}");
    Console.WriteLine($"TOTAL:    {PriceFormatter.Format(result.TotalPence)}");
    return 0;
}
catch (Exception ex) when (ex is UnknownItemException or ArgumentException)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

static string[] ReadItemsFromConsole()
{
    Console.Write("Enter items separated by commas (e.g. Apple, Apple, Banana): ");
    var input = Console.ReadLine() ?? string.Empty;
    return input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
