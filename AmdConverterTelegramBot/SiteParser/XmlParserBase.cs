using System.Globalization;
using System.Xml.Linq;
using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.SiteParser;

public abstract class XmlParserBase : ApiRateParserBase
{
    protected XmlParserBase(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    public override Result<ExchangePoint> Parse(string xmlString, bool cash)
    {
        var xmlDoc = XElement.Parse(xmlString);
        
        if (xmlDoc.FirstNode == null)
            return Result<ExchangePoint>.Error("Site is unavailable");
        
        return Result<ExchangePoint>.Ok(Parse(xmlDoc, cash));
    }

    public ExchangePoint Parse(XElement document, bool cash)
    {
        var exchangePoint = CreateExchangePoint();
        foreach (var rateNode in document.Descendants())
        {
            if (!rateNode.HasAttributes) continue;
            
            if (_currencyParser.TryParse(ExtractCurrency(rateNode), out var currency))
            {
                if (TryParseRate(BuyRate(rateNode), out Rate buy))
                {
                    exchangePoint.AddRate(new Conversion {From = currency!, To = Currency.Amd}, buy);
                }
                
                if (TryParseRate(SellRate(rateNode), out Rate sell))
                {
                    exchangePoint.AddRate(new Conversion {From = Currency.Amd, To = currency!}, sell);
                }
            }
        }
        
        return exchangePoint;
    }

    protected abstract string ExtractCurrency(XElement element);
    protected abstract string BuyRate(XElement element);
    protected abstract string SellRate(XElement element);
    
}