using System.Globalization;
using AmdConverterTelegramBot.Entities;
using Newtonsoft.Json;

namespace AmdConverterTelegramBot.SiteParser;

public abstract class JsonApiRateParser : ApiRateParserBase
{
    protected JsonApiRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }
    
    public override Result<ExchangePoint> Parse(string json, bool cash)
    {
        if (!cash && !NonCashOperationAllowed)
            return Result<ExchangePoint>.Error("Only cash operations are supported");
        
        var exchangePoint = CreateExchangePoint();

        dynamic result = JsonConvert.DeserializeObject(json);

        foreach (var rate in Rates(result))
        {
            if (_currencyParser.TryParse(ExtractCurrency(rate), out Currency currency))
            {
                if (TryParseRate(ExtractBuyRate(rate, cash), out Rate buy))
                {
                    exchangePoint.AddRate(new Conversion {From = currency!, To = Currency.Amd}, buy);
                }
                
                if (TryParseRate(ExtractSellRate(rate, cash), out Rate sell))
                {
                    exchangePoint.AddRate(new Conversion {From = Currency.Amd, To = currency!}, sell);
                }
            }
        }
        
        return Result<ExchangePoint>.Ok(exchangePoint);
    }

    protected abstract dynamic Rates(dynamic json);
    protected abstract string ExtractCurrency(dynamic rate);
    protected abstract string ExtractBuyRate(dynamic rate, bool cash);
    protected abstract string ExtractSellRate(dynamic rate, bool cash);
}