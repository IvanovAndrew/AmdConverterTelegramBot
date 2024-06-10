using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class ConverseRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Converse bank";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ConverseRateParser(currencyParser, cultureInfo);
    }
}