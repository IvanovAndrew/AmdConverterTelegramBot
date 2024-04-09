using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class InecobankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Inecobank";
    protected override string Site => "https://www.inecobank.am/api/rates";

    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new InecobankRateParser(new CurrencyParser(), CultureInfo.InvariantCulture);

    }
}