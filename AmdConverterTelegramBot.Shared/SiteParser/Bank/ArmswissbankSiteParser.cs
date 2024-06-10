using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

public class ArmswissbankSiteParser : JsonApiRateParser
{
    internal override string Url => "https://www.armswissbank.am/include/ajax.php?asd";
    protected override string ExchangeName => "Armswissbank";
    public ArmswissbankSiteParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override dynamic Rates(dynamic json, bool cash) => json["lmasbrate"];

    protected override string ExtractCurrency(dynamic rate) => rate.ISO.ToString();

    protected override string ExtractBuyRate(dynamic rate, bool cash) =>
        cash ? rate.BID_cash.ToString() : rate.BID.ToString(CultureInfo);

    protected override string ExtractSellRate(dynamic rate, bool cash) =>
        cash ? rate.OFFER_cash.ToString() : rate.OFFER.ToString(CultureInfo);

    
}