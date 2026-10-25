using ShoppingBasket.Core.Pricing;

namespace ShoppingBasket.Tests;

/// <summary>Unit tests for each pricing rule on its own, independent of the catalogue.</summary>
public class PricingRuleTests
{
    // ---------- UnitPricingRule ----------

    [Theory]
    [InlineData(0, 35, 0)]
    [InlineData(1, 35, 35)]
    [InlineData(4, 20, 80)]
    public void Unit_rule_charges_quantity_times_price(int qty, int price, int expected)
    {
        UnitPricingRule.Instance.CalculatePence(qty, price).Should().Be(expected);
    }

    // ---------- MultiBuyPricingRule ----------

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 2)]
    [InlineData(7, 4)]
    public void Buy_one_get_one_free_charges_for_half_rounded_up(int qty, int chargedItems)
    {
        MultiBuyPricingRule.BuyOneGetOneFree().CalculatePence(qty, 10).Should().Be(chargedItems * 10);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    [InlineData(5, 4)]
    [InlineData(6, 4)]
    public void Three_for_two_charges_two_of_every_three(int qty, int chargedItems)
    {
        MultiBuyPricingRule.ThreeForTwo().CalculatePence(qty, 10).Should().Be(chargedItems * 10);
    }

    [Fact]
    public void Custom_multibuy_rule_works_for_other_offers()
    {
        // "5 for the price of 3"
        var rule = new MultiBuyPricingRule(5, 3, "5 for 3");
        rule.CalculatePence(11, 10).Should().Be((3 + 3 + 1) * 10);
    }

    [Fact]
    public void Group_size_of_one_with_one_charged_is_full_price()
    {
        new MultiBuyPricingRule(1, 1, "none").CalculatePence(5, 10).Should().Be(50);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(2, 3)]
    [InlineData(2, -1)]
    public void Invalid_multibuy_configuration_is_rejected(int groupSize, int charged)
    {
        var act = () => new MultiBuyPricingRule(groupSize, charged, "bad");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Multibuy_requires_a_description()
    {
        var act = () => new MultiBuyPricingRule(2, 1, " ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Negative_quantity_is_rejected()
    {
        var act = () => MultiBuyPricingRule.ThreeForTwo().CalculatePence(-1, 15);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Negative_price_is_rejected()
    {
        var act = () => UnitPricingRule.Instance.CalculatePence(1, -5);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Overflow_is_detected_rather_than_wrapping()
    {
        var act = () => UnitPricingRule.Instance.CalculatePence(int.MaxValue, 2);
        act.Should().Throw<OverflowException>();
    }
}
