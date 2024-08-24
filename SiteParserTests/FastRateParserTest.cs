using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class FastRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Fast Bank";
    protected override FastRateParser CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new FastRateParser(currencyParser, cultureInfo);
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