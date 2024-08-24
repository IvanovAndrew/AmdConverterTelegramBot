using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;
using Xunit;

namespace SiteParsersTests;

public class ByblosRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Byblos bank Armenia";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ByblosRateParser(currencyParser, cultureInfo);
    }
    
    [Fact]
    public async Task ParseRublesCashRate()
    {
        await RunTest(Currency.Rur, cash:true);
    }
}