using System.Globalization;

namespace AmdConverterTelegramBot.SiteParser.Bank;

public class FastRateParser : JsonApiRateParser
{
    public FastRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Fast Bank";

    protected override dynamic Rates(dynamic json) => json["Rates"];

    protected override string ExtractCurrency(dynamic rate) => rate.Id.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) => rate.Buy.ToString(_cultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) => rate.Sale.ToString(_cultureInfo);
}