using System.Globalization;
using System.Xml.Linq;

namespace AmdConverterTelegramBot.SiteParser.Bank;

public class ArmbusinessbankRateParser : XmlParserBase
{
    public ArmbusinessbankRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Armbusinessbank";

    protected override string ExtractCurrency(XElement element) => element.Attributes("cur").First().Value;

    protected override string BuyRate(XElement element) => element.Attributes("value1").First().Value;
    protected override string SellRate(XElement element) => element.Attributes("value2").First().Value;
}