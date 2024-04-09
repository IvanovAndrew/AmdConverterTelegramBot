using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class AmeriabankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Ameriabank";
    protected override string Site => "https://ameriabank.am/en";

    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AmeriabankRatesParser(currencyParser, cultureInfo);
    }
}