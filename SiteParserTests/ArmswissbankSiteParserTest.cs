using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class ArmswissbankSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Armswissbank";

    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArmswissbankSiteParser(new CurrencyParser(), CultureInfo.InvariantCulture);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseGelRate(bool cash)
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