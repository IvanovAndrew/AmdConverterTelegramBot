using System;
using System.Globalization;
using System.Linq;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using Sprache;
using Xunit;

namespace AmdConverterTelegramBotTests;

public class RequestParserTests
{
    [Theory]
    [InlineData("1000", 1000)]
    [InlineData("10 000", 10_000)]
    [InlineData("100 000", 100_000)]
    [InlineData("1 000 000", 1_000_000)]
    [InlineData("10 000 000", 10_000_000)]
    [InlineData("100 000 000", 100_000_000)]
    [InlineData("1 000 000 000", 1_000_000_000)]
    public void ParseAmountInput(string input, decimal expected)
    {
        var response = Grammar.Request.Parse(input) as AmountState;
        Assert.Equal(expected, response.Amount);   
    }
    
    [Theory]
    [InlineData("9$", 9, "USD")]
    [InlineData("9USD", 9, "USD")]
    [InlineData("9 USD", 9, "USD")]
    public void ParseMoneyInput(string input, decimal expectedAmount, string expectedCurrency)
    {
        var response = Grammar.Request.Parse(input) as MoneyState;
        Assert.Equal(expectedCurrency, response.Money.Currency.Name);
        Assert.Equal(expectedAmount, response.Money.Amount);
    }
    
    [Theory]
    [InlineData("card 800$", false, 800, "USD")]
    [InlineData("cash 800USD", true, 800, "USD")]
    public void ParseMoneyAndWayInput(string input, bool expectedCash, decimal expectedAmount, string expectedCurrency)
    {
        var response = Grammar.Request.Parse(input) as MoneyAndWayState;
        Assert.Equal(expectedCash, response.Cash);
        Assert.Equal(expectedCurrency, response.Money.Currency.Name);
        Assert.Equal(expectedAmount, response.Money.Amount);
    }
    
    [Theory]
    [InlineData("card 1 000 000 amd to €", 1_000_000, "AMD", "AMD", "EUR", false)]
    [InlineData("Cash 100 AMD в рублях", 100, "AMD", "AMD", "RUR", true)]
    [InlineData("card 1000 amd -> RUR", 1000, "AMD", "AMD", "RUR", false)]
    [InlineData("Cash 1000amd in RUR", 1000, "AMD", "AMD", "RUR", true)]
    [InlineData("card 1 000amd в рубль", 1000, "AMD", "AMD", "RUR", false)]
    [InlineData("cash 1 000.00amd -> ₽", 1000, "AMD", "AMD", "RUR", true)]
    [InlineData("card USD->1000֏", 1000, "AMD", "USD", "AMD", false)]
    [InlineData("cash AMD->1000RUB", 1000, "RUR", "AMD", "RUR", true)]
    [InlineData("card 15000₽->AMD", 15_000, "RUR", "RUR", "AMD", false)]
    [InlineData("card 10ლ -> AMD", 10, "GEL", "GEL", "AMD", false)]
    [InlineData("cash GEL -> 15000 AMD", 15_000, "AMD", "GEL", "AMD", true)]
    public void ParseFullInput(string text, decimal amount, string moneyCurrency, string currencyFrom, string currencyTo, bool expectedCash)
    {
        var parsed = Grammar.Request.Parse(text);
        var request = parsed as FullRequest;
        Assert.NotNull(request);
        
        Assert.Equal(amount, request.Conversion.Money.Amount);
        Assert.Equal(moneyCurrency, request.Conversion.Money.Currency.Name);
        Assert.Equal(expectedCash, request.Cash);
        Assert.Equal(currencyFrom, request.Conversion.From.Name);
        Assert.Equal(currencyTo, request.Conversion.To.Name);
    }
}