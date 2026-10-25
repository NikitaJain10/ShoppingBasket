using ShoppingBasket.Core;
using ShoppingBasket.Core.Catalogue;
using ShoppingBasket.Core.Models;
using ShoppingBasket.Core.Pricing;

namespace ShoppingBasket.Tests;

public class CatalogueTests
{
    [Theory]
    [InlineData("Apple", 35)]
    [InlineData("Banana", 20)]
    [InlineData("Melon", 50)]
    [InlineData("Lime", 15)]
    public void Default_catalogue_has_prices_from_the_brief(string name, int pence)
    {
        var catalogue = InMemoryProductCatalogue.CreateDefault();

        catalogue.TryGetProduct(name, out var product).Should().BeTrue();
        product!.UnitPricePence.Should().Be(pence);
    }

    [Fact]
    public void Default_catalogue_assigns_the_right_offers()
    {
        var catalogue = InMemoryProductCatalogue.CreateDefault();

        catalogue.TryGetProduct("Melon", out var melon);
        catalogue.TryGetProduct("Lime", out var lime);
        catalogue.TryGetProduct("Apple", out var apple);

        var melonRule = melon!.PricingRule.Should().BeOfType<MultiBuyPricingRule>().Subject;
        melonRule.GroupSize.Should().Be(2);
        melonRule.ChargedItemsPerGroup.Should().Be(1);

        var limeRule = lime!.PricingRule.Should().BeOfType<MultiBuyPricingRule>().Subject;
        limeRule.GroupSize.Should().Be(3);
        limeRule.ChargedItemsPerGroup.Should().Be(2);

        apple!.PricingRule.Should().BeSameAs(UnitPricingRule.Instance);
    }

    [Fact]
    public void Unknown_product_is_not_found()
    {
        InMemoryProductCatalogue.CreateDefault().TryGetProduct("Mango", out var product).Should().BeFalse();
        product.Should().BeNull();
    }

    [Fact]
    public void Duplicate_products_are_rejected()
    {
        var act = () => new InMemoryProductCatalogue(new[]
        {
            new Product("Apple", 35, UnitPricingRule.Instance),
            new Product("apple", 40, UnitPricingRule.Instance),
        });

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Calculator_works_with_a_custom_catalogue()
    {
        // Shows the calculator is not tied to the default products.
        var catalogue = new InMemoryProductCatalogue(new[]
        {
            new Product("Orange", 40, MultiBuyPricingRule.BuyOneGetOneFree()),
        });

        new BasketCalculator(catalogue).CalculateTotalPence(new[] { "Orange", "Orange", "Orange" })
            .Should().Be(80);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Product_requires_a_name(string name)
    {
        var act = () => new Product(name, 10, UnitPricingRule.Instance);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Product_rejects_negative_price()
    {
        var act = () => new Product("Apple", -1, UnitPricingRule.Instance);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Product_requires_a_pricing_rule()
    {
        var act = () => new Product("Apple", 35, null!);
        act.Should().Throw<ArgumentNullException>();
    }
}

public class PriceFormatterTests
{
    [Theory]
    [InlineData(0, "0p")]
    [InlineData(35, "35p")]
    [InlineData(99, "99p")]
    [InlineData(100, "£1.00")]
    [InlineData(170, "£1.70")]
    [InlineData(12345, "£123.45")]
    [InlineData(-50, "-50p")]
    public void Formats_pence_for_display(int pence, string expected)
    {
        PriceFormatter.Format(pence).Should().Be(expected);
    }
}
