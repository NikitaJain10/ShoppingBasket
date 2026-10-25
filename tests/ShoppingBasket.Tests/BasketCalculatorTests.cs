using ShoppingBasket.Core;
using ShoppingBasket.Core.Catalogue;
using ShoppingBasket.Core.Exceptions;

namespace ShoppingBasket.Tests;

/// <summary>
/// End-to-end tests of the calculator against the catalogue from the brief:
/// Apple 35p, Banana 20p, Melon 50p (BOGOF), Lime 15p (3 for 2).
/// </summary>
public class BasketCalculatorTests
{
    private readonly BasketCalculator _calculator = new(InMemoryProductCatalogue.CreateDefault());

    private int Total(params string[] items) => _calculator.CalculateTotalPence(items);

    // ---------- Empty basket ----------

    [Fact]
    public void Empty_basket_costs_nothing()
    {
        Total().Should().Be(0);
    }

    [Fact]
    public void Empty_basket_has_no_lines()
    {
        _calculator.Calculate(Array.Empty<string>()).Lines.Should().BeEmpty();
    }

    // ---------- Single items at unit price ----------

    [Theory]
    [InlineData("Apple", 35)]
    [InlineData("Banana", 20)]
    [InlineData("Melon", 50)]
    [InlineData("Lime", 15)]
    public void Single_item_costs_its_unit_price(string item, int expected)
    {
        Total(item).Should().Be(expected);
    }

    // ---------- Example from the brief ----------

    [Fact]
    public void Example_from_brief_two_apples_and_a_banana_costs_90p()
    {
        Total("Apple", "Apple", "Banana").Should().Be(90);
    }

    // ---------- Items without offers scale linearly ----------

    [Theory]
    [InlineData(2, 70)]
    [InlineData(3, 105)]
    [InlineData(10, 350)]
    public void Apples_have_no_offer(int count, int expected)
    {
        Total(Repeat("Apple", count)).Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 40)]
    [InlineData(5, 100)]
    public void Bananas_have_no_offer(int count, int expected)
    {
        Total(Repeat("Banana", count)).Should().Be(expected);
    }

    // ---------- Melons: buy one get one free ----------

    [Theory]
    [InlineData(1, 50)]   // no pair yet
    [InlineData(2, 50)]   // 1 pair  -> pay for 1
    [InlineData(3, 100)]  // 1 pair + 1
    [InlineData(4, 100)]  // 2 pairs
    [InlineData(5, 150)]  // 2 pairs + 1
    [InlineData(10, 250)] // 5 pairs
    public void Melons_are_buy_one_get_one_free(int count, int expected)
    {
        Total(Repeat("Melon", count)).Should().Be(expected);
    }

    // ---------- Limes: three for the price of two ----------

    [Theory]
    [InlineData(1, 15)]
    [InlineData(2, 30)]   // not enough for the offer
    [InlineData(3, 30)]   // 1 group -> pay for 2
    [InlineData(4, 45)]   // 1 group + 1
    [InlineData(5, 60)]   // 1 group + 2
    [InlineData(6, 60)]   // 2 groups
    [InlineData(7, 75)]
    [InlineData(9, 90)]   // 3 groups
    public void Limes_are_three_for_the_price_of_two(int count, int expected)
    {
        Total(Repeat("Lime", count)).Should().Be(expected);
    }

    // ---------- Mixed baskets ----------

    [Fact]
    public void Mixed_basket_applies_each_offer_independently()
    {
        // 2 Apples 70 + 1 Banana 20 + 2 Melons 50 (BOGOF) + 3 Limes 30 (3-for-2) = 170
        Total("Apple", "Apple", "Banana", "Melon", "Melon", "Lime", "Lime", "Lime")
            .Should().Be(170);
    }

    [Fact]
    public void Offers_count_items_wherever_they_appear_in_the_list()
    {
        // Same basket as above, shuffled: order must not matter.
        Total("Lime", "Melon", "Apple", "Lime", "Banana", "Melon", "Apple", "Lime")
            .Should().Be(170);
    }

    [Fact]
    public void One_of_each_item_gets_no_discount()
    {
        Total("Apple", "Banana", "Melon", "Lime").Should().Be(120);
    }

    [Fact]
    public void Large_mixed_basket_is_priced_correctly()
    {
        var items = Repeat("Apple", 3)          // 105
            .Concat(Repeat("Banana", 4))        //  80
            .Concat(Repeat("Melon", 5))         // 150 (pay for 3)
            .Concat(Repeat("Lime", 8))          //  90 (pay for 6)
            .ToArray();

        Total(items).Should().Be(425);
    }

    [Fact]
    public void Very_large_basket_does_not_lose_precision()
    {
        Total(Repeat("Melon", 100_000)).Should().Be(2_500_000);
    }

    // ---------- Input handling ----------

    [Theory]
    [InlineData("apple")]
    [InlineData("APPLE")]
    [InlineData("  Apple  ")]
    public void Item_names_ignore_case_and_surrounding_spaces(string item)
    {
        Total(item).Should().Be(35);
    }

    [Fact]
    public void Differently_cased_names_count_towards_the_same_offer()
    {
        Total("melon", "MELON").Should().Be(50);
    }

    [Fact]
    public void Null_basket_throws_ArgumentNullException()
    {
        var act = () => _calculator.CalculateTotalPence(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Empty_item_name_throws_ArgumentException(string? item)
    {
        var act = () => _calculator.CalculateTotalPence(new[] { "Apple", item! });
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Unknown_item_throws_with_the_item_name()
    {
        var act = () => Total("Apple", "Mango");
        act.Should().Throw<UnknownItemException>()
            .Which.ItemName.Should().Be("Mango");
    }

    [Fact]
    public void Plural_name_is_treated_as_unknown()
    {
        var act = () => Total("Apples");
        act.Should().Throw<UnknownItemException>();
    }

    [Fact]
    public void Constructor_rejects_null_catalogue()
    {
        var act = () => new BasketCalculator(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ---------- Receipt breakdown ----------

    [Fact]
    public void Breakdown_groups_items_and_reports_savings()
    {
        var result = _calculator.Calculate(new[] { "Melon", "Apple", "Melon", "Lime", "Lime", "Lime" });

        result.Lines.Select(l => l.ProductName).Should().Equal("Melon", "Apple", "Lime");

        var melon = result.Lines.Single(l => l.ProductName == "Melon");
        melon.Quantity.Should().Be(2);
        melon.FullPricePence.Should().Be(100);
        melon.ChargedPence.Should().Be(50);
        melon.SavingPence.Should().Be(50);

        var lime = result.Lines.Single(l => l.ProductName == "Lime");
        lime.SavingPence.Should().Be(15);

        result.SubtotalPence.Should().Be(180);
        result.TotalSavingsPence.Should().Be(65);
        result.TotalPence.Should().Be(115);
    }

    private static string[] Repeat(string item, int count) => Enumerable.Repeat(item, count).ToArray();
}
