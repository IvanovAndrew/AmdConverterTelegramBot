using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class AcbaSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Acba bank";

    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AcbaSiteParser(currencyParser, cultureInfo);
    }
}