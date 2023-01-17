using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class EvocaSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Evocabank";
    protected override string Site => "https://www.evoca.am/en";

    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new EvocabankSiteParser(currencyParser, cultureInfo);
    }
}