using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

class HSBCRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "HSBC";
    protected override string Site => "https://www.hsbc.am/en-am/help/rates/";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new HSBCRateParser(currencyParser, cultureInfo);
    }
}