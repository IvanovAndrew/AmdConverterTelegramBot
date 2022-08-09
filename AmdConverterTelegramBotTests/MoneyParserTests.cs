using System.Collections.Generic;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using Xunit;

namespace AmdConverterTelegramBotTests;

public class MoneyParserTests
{
    private MoneyParser CreateMoneyParser()
    {
        var currencyParser = new CurrencyParser(new Dictionary<string, string>
        {
            ["amd"] = "AMD",
            ["драм"] = "AMD",
            ["драмов"] = "AMD",
            ["драма"] = "AMD",
        });
        return new MoneyParser(currencyParser, "AMD");
    }
    
    [Theory]
    [InlineData("100 amd", 100, "AMD")]
    [InlineData("1 000 000 amd", 1_000_000, "AMD")]
    public void Test(string input, decimal amount, string currency)
    {
        var parser = CreateMoneyParser();
        Assert.True(parser.TryParse(input, out Money? money));
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency.Name);
    }
}