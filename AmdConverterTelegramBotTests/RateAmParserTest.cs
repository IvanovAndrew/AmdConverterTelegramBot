using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using HtmlAgilityPack;
using Xunit;

namespace AmdConverterTelegramBotTests;

public class RateAmParserTest
{
    private HtmlDocument GetHtmlDocument()
    {
        var web = new HtmlWeb();
        return web.Load("https://rate.am/en/armenian-dram-exchange-rates/banks/non-cash");
    }
    
    [Fact]
    public void RatesFromAmdIsHigherThanRatesFromAmd()
    {
        var parser = new RateAmParser(
            new MoneyParser(new CurrencyParser(new Dictionary<string, string>()), "AMD"),
            new CultureInfo("en-us"));

        var rates = parser.Parse(GetHtmlDocument());
        
        Assert.True(rates.IsSuccess);

        var s = rates.Value.First().Rates;

        var amdToUsdConversion = new Conversion() { From = Currency.Amd, To = Currency.Usd };
        var usdToAmdConversion = new Conversion() { From = Currency.Usd, To = Currency.Amd };
        
        Assert.True(s[amdToUsdConversion].FXRate > s[usdToAmdConversion].FXRate);
    }
}