using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class ArmswissbankSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Armswissbank";
    protected override string Site => "https://www.armswissbank.am/include/ajax.php?asd";

    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArmswissbankSiteParser(new CurrencyParser(), CultureInfo.InvariantCulture);
    }
}