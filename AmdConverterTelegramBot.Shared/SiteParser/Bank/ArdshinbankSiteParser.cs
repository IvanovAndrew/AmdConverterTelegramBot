using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class ArdshinbankSiteParser : JsonApiRateParser
{
    public ArdshinbankSiteParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://website-api.ardshinbank.am/currency";
    protected override string ExchangeName => "Ardshinbank";


    protected override dynamic Rates(dynamic json, bool cash) => json["data"]["currencies"][cash? "cash" : "no_cash"];
    

    protected override string ExtractCurrency(dynamic rate)
    {
        return rate["type"];
    }

    protected override string ExtractBuyRate(dynamic rate, bool cash)
    {
        return rate["buy"];
    }

    protected override string ExtractSellRate(dynamic rate, bool cash)
    {
        return rate["sell"];
    }
}