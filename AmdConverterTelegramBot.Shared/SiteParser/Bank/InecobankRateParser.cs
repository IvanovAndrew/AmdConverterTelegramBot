using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

public class InecobankRateParser : JsonApiRateParser
{
    public InecobankRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }
    
    protected override dynamic Rates(dynamic json, bool cash) => json["items"];
    protected override string ExtractCurrency(dynamic rate) => rate.code.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) =>
        rate[cash ? "cash" : "cashless"].buy.ToString(CultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) =>
        rate[cash ? "cash" : "cashless"].sell.ToString(CultureInfo);

    internal override string Url => "https://www.inecobank.am/api/rates/";
    protected override string ExchangeName => "Inecobank";
}