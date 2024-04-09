namespace AmdConverterTelegramBot.Shared.Entities;

public record Currency
{
    public static readonly Currency Eur = new ("EUR", "€");
    public static readonly Currency Usd = new ("USD", "$");
    public static readonly Currency Rur = new ("RUR", "₽");
    public static readonly Currency Amd = new ("AMD", "֏");
    //public static readonly Currency GBP = new ("GBP", "£");
    public static readonly Currency GEL = new ("GEL", "ლ");
    
    public string Name { get; private set; }
    public string Symbol { get; private set; }
    private Currency(string name, string symbol)
    {
        Name = name;
        Symbol = symbol;
    }

    public static List<Currency> GetAvailableCurrencies()
    {
        List<Currency> currencies = new List<Currency>();
        foreach (var fieldInfo in typeof(Currency).GetFields())
        {
            if (fieldInfo.FieldType == typeof(Currency))
            {
                if (fieldInfo.GetValue(null) is Currency currency)
                {
                    currencies.Add(currency);
                }
            }
        }

        return currencies;
    }

    public static char[] GetAvailableSymbols()
    {
        var availableCurrencies = GetAvailableCurrencies();

        var result = new char[availableCurrencies.Count];
        int i = 0;
        foreach (var currency in availableCurrencies)
        {
            result[i++] = currency.Symbol[0];
        }

        return result;
    }

    public override string ToString()
    {
        return Symbol;
    }
}