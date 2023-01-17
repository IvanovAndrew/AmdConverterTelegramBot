using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class ArmswissbankSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Armswissbank";
    protected override string Site => "https://www.armswissbank.am/include/ajax.php?asd";

    protected override RateParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArmswissbankSiteParser(new CurrencyParser(new Dictionary<string, string>() {["RUB"] = "RUR"}),
            CultureInfo.InvariantCulture);
    }
}