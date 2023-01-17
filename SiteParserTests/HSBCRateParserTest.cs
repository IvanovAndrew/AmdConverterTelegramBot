using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

class HSBCRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "HSBC";
    protected override string Site => "https://www.hsbc.am/en-am/help/rates/";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new HSBCRateParser(currencyParser, cultureInfo);
    }
}