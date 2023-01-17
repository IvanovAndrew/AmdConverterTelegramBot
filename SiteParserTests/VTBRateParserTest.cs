using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class VTBRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "VTB Bank (Armenia)";
    protected override string Site => "https://www.vtb.am/ru/currency";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new VTBRateParser(currencyParser, cultureInfo);
    }
}