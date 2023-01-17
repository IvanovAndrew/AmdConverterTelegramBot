using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class ArmbusinessbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Armbusinessbank";
    protected override string Site => "https://www.armbusinessbank.am/";

    protected override RateParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArmbusinessbankRateParser(new CurrencyParser(new Dictionary<string, string>() {["RUB"] = "RUR"}),
            CultureInfo.InvariantCulture);
    }

    protected override ExchangePoint Execute(bool cash)
    {
        var parser = new ArmbusinessbankRateParser(new CurrencyParser(new Dictionary<string, string>() {["RUB"] = "RUR"}),
            CultureInfo.InvariantCulture);

        string url = $"https://www.armbusinessbank.am/rates/Rates{(cash ? 991 : 990)}.xml?timestamp={DateTimeOffset.UnixEpoch}";

        var xmlDoc = GetString(url);

        var exchangePoint = parser.Parse(xmlDoc, cash);
        return exchangePoint.Value;
    }
}