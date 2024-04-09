using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

public class MellatbankRateParser : JsonApiRateParser
{
    public MellatbankRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url { get; }
    protected override string ExchangeName => "Mellat bank";
    protected override dynamic Rates(dynamic json) => json["result"]["data"];

    protected override string ExtractCurrency(dynamic rate) => rate.currency.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) => (cash ? rate.buyCash : rate.buy).ToString(_cultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) => (cash ? rate.sellCash : rate.sell).ToString(_cultureInfo);
}