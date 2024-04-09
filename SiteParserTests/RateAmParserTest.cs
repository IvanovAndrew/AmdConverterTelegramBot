using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using HtmlAgilityPack;
using Xunit;

namespace SiteParsersTests;

public class RateAmParserTest
{
    private HtmlDocument GetHtmlDocument()
    {
        var web = new HtmlWeb();
        return web.Load("https://www.rate.am/en/armenian-dram-exchange-rates/banks");
    }
    
    [Fact]
    public void ParseNonCashRates()
    {
        var parser = new RateAmParser(
            new CurrencyParser(),
            new CultureInfo("en-us"));

        // Act
        var rates = parser.Parse(GetHtmlDocument(), false);
        
        // Assert
        Assert.True(rates.IsSuccess);
    }
    
    [Fact]
    public void ParseCashRates()
    {
        var parser = new RateAmParser(
            new CurrencyParser(),
            new CultureInfo("en-us"));

        // Act
        var rates = parser.Parse(GetHtmlDocument(), true);
        
        // Assert
        Assert.True(rates.IsSuccess);
    }
    
    [Theory, MemberData(nameof(ForeignCurrency))]
    public void BuyRateIsLowerThanSellRate(Currency currency)
    {
        var parser = new RateAmParser(
            new CurrencyParser(),
            new CultureInfo("en-us"));

        var rates = parser.Parse(GetHtmlDocument(), true);
        
        Assert.True(rates.IsSuccess);

        var s = rates.Value.First().Rates;

        var amdToUsdConversion = new Conversion() { From = Currency.Amd, To = Currency.Usd };
        var usdToAmdConversion = new Conversion() { From = Currency.Usd, To = Currency.Amd };
        
        Assert.True(s[amdToUsdConversion].FXRate > s[usdToAmdConversion].FXRate);
    }
    
    public static IEnumerable<object[]> ForeignCurrency => new[]
    {
        new [] {Currency.Eur}, 
        new []{Currency.Rur}, 
        new []{Currency.Usd},
    };
}