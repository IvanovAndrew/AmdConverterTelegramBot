using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class AmioBankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Amiobank";
    protected override string Site => "https://www.amiobank.am/en";

    protected override RateParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AmioBankRateParser(new CurrencyParser(new Dictionary<string, string>() {["RUB"] = "RUR"}),
            CultureInfo.InvariantCulture);
    }

    protected override ExchangePoint Execute(bool cash)
    {
        var parser = new AmioBankRateParser(new CurrencyParser(new Dictionary<string, string>() {["RUB"] = "RUR"}),
            CultureInfo.InvariantCulture);

        string url = Site;

        var xmlDoc = GetString(url);

        var exchangePoint = parser.Parse(xmlDoc, cash);
        return exchangePoint.Value;
    }
}