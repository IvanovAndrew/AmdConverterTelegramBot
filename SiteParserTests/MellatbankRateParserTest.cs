using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class MellatbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Mellat bank";
    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new MellatbankRateParser(currencyParser, cultureInfo);
    }
}