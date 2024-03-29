using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class ByblosRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Byblos bank Armenia";
    protected override string Site => "https://www.byblosbankarmenia.am/en";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ByblosRateParser(currencyParser, cultureInfo);
    }
}