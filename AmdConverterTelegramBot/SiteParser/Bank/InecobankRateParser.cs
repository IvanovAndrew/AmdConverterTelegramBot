using System.Globalization;
using AmdConverterTelegramBot.Services;

namespace AmdConverterTelegramBot.SiteParser.Bank;

public class InecobankRateParser : JsonApiRateParser
{
    public InecobankRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }
    
    protected override dynamic Rates(dynamic json) => json["items"];
    protected override string ExtractCurrency(dynamic rate) => rate.code.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) =>
        rate[cash ? "cash" : "cashless"].buy.ToString(_cultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) =>
        rate[cash ? "cash" : "cashless"].sell.ToString(_cultureInfo);

    protected override string ExchangeName => "Inecobank";
}