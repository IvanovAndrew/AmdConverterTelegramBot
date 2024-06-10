using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using Xunit;

namespace SiteParsersTests;

public abstract class ArmenianBankSiteBaseTest
{
    protected abstract string BankName { get; }

    protected abstract RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo);
    
    [Fact]
    public async Task ParseCashRates()
    {
        // Act
        var exchangePoint = await Execute(true);
            
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
    public async Task ParseNonCashRates()
    {
        // Act
        var exchangePoint = await Execute(false);
            
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
    public async Task BuyRateIsLowerThanSellRate(Currency currency)
    {
        // Act
        var exchangePoint = await Execute(true);
            
        // Assert
        var currencyToAmdRate =
            exchangePoint.Rates.First(c => c.Key == new Conversion { From = currency, To = Currency.Amd }).Value;
        var amdToCurrencyRate =
            exchangePoint.Rates.First(c => c.Key == new Conversion { From = Currency.Amd, To = currency }).Value;
        
        Assert.True(currencyToAmdRate.FXRate < amdToCurrencyRate.FXRate);
    }

    public static IEnumerable<object[]> ForeignCurrency => new[]
    {
        new []{Currency.Eur}, 
        new []{Currency.Rur}, 
        new []{Currency.Usd},
    };
        
    protected virtual async Task<ExchangePoint> Execute(bool cash)
    {
        var bankRateLoader = new BankRateLoader(CreateParser(new CurrencyParser(), CultureInfo.InvariantCulture));

        using (HttpClient httpClient = new HttpClient(new HttpClientHandler{AllowAutoRedirect = true, MaxAutomaticRedirections = 2}))
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");
            var exchangePoint = await bankRateLoader.GetExchangePointAsync(httpClient, cash);
            return exchangePoint.Value;
        }
    }

    protected virtual string GetString(string uri)
    {
        using (HttpClient httpClient = new HttpClient(new HttpClientHandler{AllowAutoRedirect = true, MaxAutomaticRedirections = 2}))
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            
            var response = httpClient.Send(request);
            response.EnsureSuccessStatusCode();

            using var reader = new StreamReader(response.Content.ReadAsStream());
            
            return reader.ReadToEnd();
        }
    }
}