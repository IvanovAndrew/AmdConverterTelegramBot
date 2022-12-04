using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using Xunit;

namespace AmdConverterTelegramBotTests;

public class RateAmParserTest
{
    [Fact]
    public async Task RatesFromAmdIsHigherThanRatesFromAmd()
    {
        var parser = new RateAmParser(
            new MoneyParser(new CurrencyParser(new Dictionary<string, string>()), "AMD"),
            new CultureInfo("en-us"));

        var rates = await parser.ParseAsync("https://rate.am/en/armenian-dram-exchange-rates/banks/non-cash");
        
        Assert.True(rates.IsSuccess);

        var s = rates.Value.First().Rates;

        var amdToUsdConversion = new Conversion() { From = Currency.Amd, To = Currency.Usd };
        var usdToAmdConversion = new Conversion() { From = Currency.Usd, To = Currency.Amd };
        
        Assert.True(s[amdToUsdConversion].FXRate > s[usdToAmdConversion].FXRate);
    }
}