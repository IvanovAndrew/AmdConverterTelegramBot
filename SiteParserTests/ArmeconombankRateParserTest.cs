using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class ArmeconombankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Armeconombank";
    protected override string Site => "https://www.aeb.am/en/";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArmeconombankRateParser(currencyParser, cultureInfo);
    }
}