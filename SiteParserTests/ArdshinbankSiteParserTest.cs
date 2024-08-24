using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class ArdshinbankSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Ardshinbank";

    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) =>
        new ArdshinbankSiteParser(currencyParser, cultureInfo);

    [Fact]
    public async Task ParseNonCashGelRate()
    {
        await RunTest(Currency.GEL, cash:false);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseRublesRate(bool cash)
    {
        await RunTest(Currency.Rur, cash);
    }
}