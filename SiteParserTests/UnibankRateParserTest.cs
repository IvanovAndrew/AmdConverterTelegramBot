using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class UnibankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Unibank";
    protected override string Site => "https://www.unibank.am/en/";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new UnibankRateParser(currencyParser, cultureInfo);
    }
}