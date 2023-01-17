using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class ArtsakhbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Artsakhbank";
    protected override string Site => "https://www.artsakhbank.am/en/home";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArtsakhbankRateParser(currencyParser, cultureInfo);
    }
}