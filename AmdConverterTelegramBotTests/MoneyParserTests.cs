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
            ["армянских драм"] = "AMD",
            ["армянских драмов"] = "AMD",
            ["лари"] = "GEL",
            ["грузинский лари"] = "GEL",
            ["грузинских лари"] = "GEL"
        });
        return new MoneyParser(currencyParser);
    }
    
    [Theory]
    [InlineData("10 драм", 10, "AMD")]
    [InlineData("100 армянских драм", 100, "AMD")]
    [InlineData("1000 amd", 1000, "AMD")]
    [InlineData("10 000 AMD", 10_000, "AMD")]
    [InlineData("100 000 ДРАМ", 100_000, "AMD")]
    [InlineData("1 000 000 amd", 1_000_000, "AMD")]
    [InlineData("5ლ", 5, "GEL")]
    [InlineData("15 лари", 15, "GEL")]
    public void Test(string input, decimal amount, string currency)
    {
        var parser = CreateMoneyParser();
        Assert.True(parser.TryParse(input, out Money money));
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency.Name);
    }
}