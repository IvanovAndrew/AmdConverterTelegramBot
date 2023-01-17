using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class AcbaSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Acba bank";
    protected override string Site => "https://www.acba.am/en/";

    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AcbaSiteParser(currencyParser, cultureInfo);
    }
}