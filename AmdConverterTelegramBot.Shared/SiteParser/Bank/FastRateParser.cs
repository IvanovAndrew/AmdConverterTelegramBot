using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

public class FastRateParser : JsonApiRateParser
{
    public FastRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url { get; }
    protected override string ExchangeName => "Fast Bank";

    protected override dynamic Rates(dynamic json) => json["Rates"];

    protected override string ExtractCurrency(dynamic rate) => rate.Id.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) => rate.Buy.ToString(_cultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) => rate.Sale.ToString(_cultureInfo);
}