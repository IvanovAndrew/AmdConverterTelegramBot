using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class InecobankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Inecobank";
    protected override string Site => "https://www.inecobank.am/api/rates";

    protected override RateParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new InecobankRateParser(new CurrencyParser(new Dictionary<string, string>() {["RUB"] = "RUR"}),
            CultureInfo.InvariantCulture);

    }
}