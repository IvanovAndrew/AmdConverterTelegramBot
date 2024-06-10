using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class UnibankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Unibank";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new UnibankRateParser(currencyParser, cultureInfo);
    }
}