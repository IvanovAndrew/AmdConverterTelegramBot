using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class ArdshinbankSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Ardshinbank";
    protected override string Site => "https://www.ardshinbank.am/en";

    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) =>
        new ArdshinbankSiteParser(currencyParser, cultureInfo);
}