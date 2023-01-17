using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.SiteParser;
using Xunit;

namespace SiteParsersTests;

public abstract class ArmenianBankSiteBaseTest
{
    protected abstract string BankName { get; }
    protected abstract string Site { get; }

    protected abstract RateParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo);
    
    [Fact]
    public void ParseCashRates()
    {
        // Act
        var exchangePoint = Execute(true);
            
        // Assert
        Assert.Equal(BankName, exchangePoint.Name);
        Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Usd}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Eur}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Rur}, exchangePoint.Rates.Keys);
            
        Assert.Contains(new Conversion(){From = Currency.Usd, To = Currency.Amd}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Eur, To = Currency.Amd}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Rur, To = Currency.Amd}, exchangePoint.Rates.Keys);
    }
    
    [Fact]
    public void ParseNonCashRates()
    {
        // Act
        var exchangePoint = Execute(false);
            
        // Assert
        Assert.Equal(BankName, exchangePoint.Name);
        Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Usd}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Eur}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Rur}, exchangePoint.Rates.Keys);
            
        Assert.Contains(new Conversion(){From = Currency.Usd, To = Currency.Amd}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Eur, To = Currency.Amd}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = Currency.Rur, To = Currency.Amd}, exchangePoint.Rates.Keys);
    }

    [Theory, MemberData(nameof(ForeignCurrency))]
    public void BuyRateIsLowerThanSellRate(Currency currency)
    {
        // Act
        var exchangePoint = Execute(true);
            
        // Assert
        var currencyToAmdRate =
            exchangePoint.Rates.First(c => c.Key == new Conversion { From = currency, To = Currency.Amd }).Value;
        var amdToCurrencyRate =
            exchangePoint.Rates.First(c => c.Key == new Conversion { From = Currency.Amd, To = currency }).Value;
        
        Assert.True(currencyToAmdRate.FXRate < amdToCurrencyRate.FXRate);
    }

    public static IEnumerable<object[]> ForeignCurrency => new[]
    {
        new [] {Currency.Eur}, 
        new []{Currency.Rur}, 
        new []{Currency.Usd},
    };
        
    protected virtual ExchangePoint Execute(bool cash)
    {
        var parser = CreateParser(new CurrencyParser(new Dictionary<string, string>() {["RUB"] = "RUR"}),
            CultureInfo.InvariantCulture);

        var text = GetString(Site);

        var exchangePoint = parser.Parse(text, cash);
        return exchangePoint.Value;
    }

    protected virtual string GetString(string uri)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = httpClient.Send(request);
            response.EnsureSuccessStatusCode();

            using var reader = new StreamReader(response.Content.ReadAsStream());
            
            return reader.ReadToEnd();
        }
    }
}