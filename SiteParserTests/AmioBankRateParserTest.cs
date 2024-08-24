using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class AmioBankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Amiobank";

    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AmioBankRateParser(new CurrencyParser(),
            CultureInfo.InvariantCulture);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseRublesRate(bool cash)
    {
        await RunTest(Currency.Rur, cash);
    }
}