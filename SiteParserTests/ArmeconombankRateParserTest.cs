using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class ArmeconombankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Armeconombank";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArmeconombankRateParser(currencyParser, cultureInfo);
    }
}