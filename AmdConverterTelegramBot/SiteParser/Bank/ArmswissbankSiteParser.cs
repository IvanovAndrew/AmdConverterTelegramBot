using System.Globalization;

namespace AmdConverterTelegramBot.SiteParser.Bank;

public class ArmswissbankSiteParser : JsonApiRateParser
{
    protected override string ExchangeName => "Armswissbank";
    public ArmswissbankSiteParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override dynamic Rates(dynamic json) => json["lmasbrate"];

    protected override string ExtractCurrency(dynamic rate) => rate.ISO.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) =>
        cash ? rate.BID_cash.ToString() : rate.BID.ToString(_cultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) =>
        cash ? rate.OFFER_cash.ToString() : rate.OFFER.ToString(_cultureInfo);

    
}