using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class MellatbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Mellat bank";
    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new MellatbankRateParser(currencyParser, cultureInfo);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseRublesRate(bool cash)
    {
        await RunTest(Currency.Rur, cash);
    }
}