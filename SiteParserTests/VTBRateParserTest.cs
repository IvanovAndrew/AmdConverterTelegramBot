using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class VTBRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "VTB Bank (Armenia)";
    protected override string Site => "https://www.vtb.am/ru/currency";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new VTBRateParser(currencyParser, cultureInfo);
    }
}