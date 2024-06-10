using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class AraratbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Araratbank";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AraratbankRateParser(currencyParser, cultureInfo);
    }
}