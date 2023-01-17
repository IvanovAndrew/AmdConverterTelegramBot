using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class AraratbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Araratbank";
    protected override string Site => "https://www.araratbank.am/en/";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AraratbankRateParser(currencyParser, cultureInfo);
    }
}