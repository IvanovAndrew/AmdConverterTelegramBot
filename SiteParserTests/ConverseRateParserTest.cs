using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class ConverseRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Converse bank";
    protected override string Site => "https://www.conversebank.am/en/exchange-rate/";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ConverseRateParser(currencyParser, cultureInfo);
    }
}