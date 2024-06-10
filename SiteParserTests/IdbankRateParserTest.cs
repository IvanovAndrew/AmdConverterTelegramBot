using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class IdbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "IDbank";
    protected override HtmlParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new IdbankRateParser(currencyParser, cultureInfo);
    }
}