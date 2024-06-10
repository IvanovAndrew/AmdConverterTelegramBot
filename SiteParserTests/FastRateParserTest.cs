using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class FastRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Fast Bank";
    protected override FastRateParser CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new FastRateParser(currencyParser, cultureInfo);
    }
}