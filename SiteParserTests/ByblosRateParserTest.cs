using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class ByblosRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Byblos bank Armenia";
    protected override string Site => "https://www.byblosbankarmenia.am/en";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ByblosRateParser(currencyParser, cultureInfo);
    }
}