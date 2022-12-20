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
            ["rub"] = "RUR",
            ["рубль"] = "RUR",
            ["рублях"] = "RUR",
        });
        return new RequestParser(new MoneyParser(currencyParser, "AMD"), currencyParser,
            new[] { "в ", "->", " in ", " as ", " to " });
    }
    
    [Theory]
    [InlineData("Non Cash 1 000 000 amd to €", 1_000_000, "AMD", "AMD", "EUR", false)]
    [InlineData("Cash 100 AMD в рублях", 100, "AMD", "AMD", "RUR", true)]
    [InlineData("Non cash 1000 amd non cash -> RUR", 1000, "AMD", "AMD", "RUR", false)]
    [InlineData("Cash 1000amd in RUR", 1000, "AMD", "AMD", "RUR", true)]
    [InlineData("non cash 1 000amd в рубль", 1000, "AMD", "AMD", "RUR", false)]
    [InlineData("cash 1 000.00amd -> ₽", 1000, "AMD", "AMD", "RUR", true)]
    [InlineData("non cash USD->1000֏", 1000, "AMD", "USD", "AMD", false)]
    [InlineData("cash AMD->1000RUB", 1000, "RUR", "AMD", "RUR", true)]
    [InlineData("non cash 15000₽->AMD", 15_000, "RUR", "RUR", "AMD", false)]
    public void ParseInput(string text, decimal amount, string moneyCurrency, string currencyFrom, string currencyTo, bool expectedCash)
    {
        var parser = CreateRequestParser();
        Assert.True(parser.TryParseFullRequest(text, out Money money, out bool cash, out Conversion conversion));
        Assert.Equal(amount, money.Amount);
        Assert.Equal(moneyCurrency, money.Currency?.Name);
        Assert.Equal(expectedCash, cash);
        Assert.Equal(currencyFrom, conversion.From?.Name);
        Assert.Equal(currencyTo, conversion.To.Name);
    }
}