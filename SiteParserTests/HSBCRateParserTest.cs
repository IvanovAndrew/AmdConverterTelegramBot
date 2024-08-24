using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class HSBCRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "HSBC Bank Armenia";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new HSBCRateParser(currencyParser, cultureInfo);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ParseRublesRate(bool cash)
    {
        await RunTest(Currency.Rur, cash);
    }
}