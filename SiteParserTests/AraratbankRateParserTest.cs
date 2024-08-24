using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class AraratbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Araratbank";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AraratbankRateParser(currencyParser, cultureInfo);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseNonCashGelRate(bool cash)
    {
        await RunTest(Currency.GEL, cash);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseRublesRate(bool cash)
    {
        await RunTest(Currency.Rur, cash);
    }
}