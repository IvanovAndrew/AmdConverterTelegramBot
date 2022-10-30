namespace AmdConverterTelegramBot.Entities;

public class Currency
{
    public static readonly Currency Eur = new ("EUR", "€");
    public static readonly Currency Usd = new ("USD", "$");
    public static readonly Currency Rur = new ("RUR", "₽");
    public static readonly Currency Amd = new ("AMD", "֏");
    public static readonly Currency GBP = new ("GBP", "£");
    
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
}