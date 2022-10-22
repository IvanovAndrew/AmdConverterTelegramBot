using System.Collections.Generic;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using Xunit;

namespace AmdConverterTelegramBotTests;

public class RequestParserTests
{
    private RequestParser CreateRequestParser()
    {
        var currencyParser = new CurrencyParser(new Dictionary<string, string>
        {
            ["amd"] = "AMD",
            ["армянский драм"] = "AMD",
            ["армянских драм"] = "AMD",
            ["армянских драма"] = "AMD",
            ["армянских драмов"] = "AMD",
            ["usd"] = "USD",
            ["dollar"] = "USD",
            ["доллар"] = "USD",
            ["долларах"] = "USD",
            ["долларах"] = "USD",
            ["eur"] = "EUR",
            ["euro"] = "EUR",
            ["евро"] = "EUR",
            ["rur"] = "RUR",
            ["рубль"] = "RUR",
            ["рублях"] = "RUR",
        });
        return new RequestParser(new MoneyParser(currencyParser, "AMD"), currencyParser,
            new[] { "в ", "->", " in ", " as ", " to " });
    }
    
    [Theory]
    [InlineData("Non Cash 1 000 000 amd to €", 1000000, "AMD", "EUR", false)]
    [InlineData("Cash 100 AMD в рублях", 100, "AMD", "RUR", true)]
    [InlineData("Non cash 1000 amd non cash -> RUR", 1000, "AMD", "RUR", false)]
    [InlineData("Cash 1000amd in RUR", 1000, "AMD", "RUR", true)]
    [InlineData("non cash 1 000amd в рубль", 1000, "AMD", "RUR", false)]
    [InlineData("cash 1 000.00amd -> ₽", 1000, "AMD", "RUR", true)]
    [InlineData("non cash 1000$", 1000, "USD", null, false)]
    [InlineData("non cash USD->1000֏", 1000, "USD", "AMD", false)]
    public void ParseInput(string text, decimal amount, string currencyFrom, string currencyTo, bool? expectedCash)
    {
        var parser = CreateRequestParser();
        Assert.True(parser.TryParse(text, out Money money, out bool? cash, out Currency currency, out var toCurrency));
        Assert.Equal(amount, money.Amount);
        Assert.Equal(expectedCash, cash);
        Assert.Equal(currencyFrom, (toCurrency?? false)? money.Currency.Name : currency?.Name);
        Assert.Equal(currencyTo, (toCurrency?? false)? currency?.Name : money.Currency.Name);
    }
}