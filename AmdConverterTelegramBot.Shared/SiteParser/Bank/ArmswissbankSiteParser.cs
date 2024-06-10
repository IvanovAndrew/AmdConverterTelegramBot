using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

public class ArmswissbankSiteParser : JsonApiRateParser
{
    internal override string Url { get; }
    protected override string ExchangeName => "Armswissbank";
    public ArmswissbankSiteParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override dynamic Rates(dynamic json, bool cash) => json["lmasbrate"];

    protected override string ExtractCurrency(dynamic rate) => rate.ISO.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) =>
        cash ? rate.BID_cash.ToString() : rate.BID.ToString(_cultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) =>
        cash ? rate.OFFER_cash.ToString() : rate.OFFER.ToString(_cultureInfo);

    
}