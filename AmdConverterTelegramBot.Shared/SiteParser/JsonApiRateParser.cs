using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared.Entities;
using Newtonsoft.Json;

namespace AmdConverterTelegramBot.Shared.SiteParser;

public abstract class JsonApiRateParser : ApiRateParserBase
{
    protected JsonApiRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }
    
    public override Result<ExchangePoint> Parse(string json, bool cash)
    {
        if (!cash && !CardOperationAllowed)
            return Result<ExchangePoint>.Error("Only cash operations are supported");
        
        var exchangePoint = CreateExchangePoint();

        dynamic response = JsonConvert.DeserializeObject(json);

        foreach (var rate in Rates(response, cash))
        {
            if (_currencyParser.TryParse(ExtractCurrency(rate), out Currency currency))
            {
                if (Rate.TryParse(ExtractBuyRate(rate, cash), out Rate buy))
                {
                    exchangePoint.AddRate(new Conversion {From = currency!, To = Currency.Amd}, buy);
                }
            
                if (Rate.TryParse(ExtractSellRate(rate, cash), out Rate sell))
                {
                    exchangePoint.AddRate(new Conversion {From = Currency.Amd, To = currency!}, sell);
                }
            }
        }
        
        return Result<ExchangePoint>.Ok(exchangePoint);
    }

    protected abstract dynamic Rates(dynamic json, bool cash);
    protected abstract string ExtractCurrency(dynamic rate);
    protected abstract string ExtractBuyRate(dynamic rate, bool cash);
    protected abstract string ExtractSellRate(dynamic rate, bool cash);
}