using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot;

public interface ICurrencyParser
{
    bool TryParse(string text, out Currency currency);
}

public class CurrencyParser : ICurrencyParser
{
    private readonly Dictionary<string, string> _synonyms;
        
    public CurrencyParser(Dictionary<string, string> synonyms)
    {
        _synonyms = synonyms;
    }
    
    public bool TryParse(string text, out Currency currency)
    {
        var availableCurrencies = Currency.GetAvailableCurrencies();
        var currencies = availableCurrencies.ToDictionary(c => c.Name);
        
        var str = text.Trim().ToUpperInvariant();
        
        if (currencies.TryGetValue(str, out currency))
        {
            return true;
        }
        
        currency = availableCurrencies.FirstOrDefault(c => string.Equals(c.Symbol, str, StringComparison.InvariantCultureIgnoreCase));

        if (currency != null)
        {
            return true;
        }
        
        foreach (var (word, currencyName) in _synonyms)
        {
            if (str == word.ToUpperInvariant())
            {
                currency = currencies[currencyName];
                return true;
            }
        }
        
        currency ??= Currency.Amd;
        
        return false;
    }
}