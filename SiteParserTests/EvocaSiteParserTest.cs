using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class EvocaSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Evocabank";

    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new EvocabankSiteParser(currencyParser, cultureInfo);
    }
}