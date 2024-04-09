using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.Shared.Entities;

public class ExchangePoint
{
    public string Name { get; init; }
    private readonly Dictionary<Conversion, Rate> _ratesDict = new();

    public IReadOnlyDictionary<Conversion, Rate> Rates => _ratesDict;
    public Currency BaseCurrency { get; init; }

    public ConversionInfo? Convert(Conversion conversion, Money money)
    {
        if (Rates.TryGetValue(conversion, out var rate) && rate != Rate.Unknown)
        {
            Money calculatedMoney;
            Currency requestedCurrency = conversion.From == money.Currency ? conversion.To : conversion.From;

            if (money.Currency == BaseCurrency)
            {
                var newAmount = money.Amount / rate.FXRate;
                calculatedMoney = new Money {Currency = requestedCurrency, Amount = newAmount};
            }
            else
            {
                var newAmount = money.Amount * rate.FXRate;
                calculatedMoney = new Money {Currency = requestedCurrency, Amount = newAmount};
            }
            
            return new ConversionInfo
            {
                ExchangePoint = this,
                Conversion = conversion,
                Rate = rate,
                FromMoney = money.Currency == conversion.From? money : calculatedMoney, 
                ToMoney = money.Currency == conversion.To? money : calculatedMoney,
            };
        }

        return null;
    }

    public void AddRate(Conversion conversion, Rate rate)
    {
        if (conversion.From == conversion.To) return;
        
        if (_ratesDict.ContainsKey(conversion))
            throw new ArgumentException($"Conversion {conversion} is already added");
        _ratesDict[conversion] = rate;
    }
}