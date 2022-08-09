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
    [InlineData("1 000 000 amd to €", 1000000, "AMD", "EUR")]
    [InlineData("100 AMD в рублях", 100, "AMD", "RUR")]
    [InlineData("1000 amd -> RUR", 1000, "AMD", "RUR")]
    [InlineData("1000amd in RUR", 1000, "AMD", "RUR")]
    [InlineData("1 000amd в рубль", 1000, "AMD", "RUR")]
    [InlineData("1 000.00amd -> ₽", 1000, "AMD", "RUR")]
    public void ParseInput(string text, decimal amount, string currencyFrom, string currencyTo)
    {
        var parser = CreateRequestParser();
        Assert.True(parser.TryParse(text, out Money money, out Currency currency, out var toCurrency));
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currencyFrom, money.Currency.Name);
        Assert.Equal(currencyTo, currency.Name);
    }
}