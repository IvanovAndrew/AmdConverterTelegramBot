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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseEuroRate(bool cash)
    {
        await RunTest(Currency.Eur, cash);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseUsdRate(bool cash)
    {
        await RunTest(Currency.Usd, cash);
    }
    
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

    protected async Task RunTest(Currency currencyToCheck, bool cash)
    {
        // Act
        var exchangePoint = await Execute(cash);
            
        // Assert
        Assert.Equal(BankName, exchangePoint.Name);
        
        Assert.Contains(new Conversion(){From = Currency.Amd, To = currencyToCheck}, exchangePoint.Rates.Keys);
        Assert.Contains(new Conversion(){From = currencyToCheck, To = Currency.Amd}, exchangePoint.Rates.Keys);
        
        var currencyToAmdRate =
            exchangePoint.Rates.First(c => c.Key == new Conversion { From = Currency.Eur, To = Currency.Amd }).Value;
        var amdToCurrencyRate =
            exchangePoint.Rates.First(c => c.Key == new Conversion { From = Currency.Amd, To = Currency.Eur }).Value;
        
        Assert.True(currencyToAmdRate.FXRate < amdToCurrencyRate.FXRate);
    }
}