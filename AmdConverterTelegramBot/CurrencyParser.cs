using System.Collections.Generic;
using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot;

public interface ICurrencyParser
{
    bool TryParse(string text, out Currency? currency);
}

public class CurrencyParser : ICurrencyParser
{
    private readonly Dictionary<string, string> _synonyms;
        
    public CurrencyParser(Dictionary<string, string> synonyms)
    {
        _synonyms = synonyms;
    }
    
    public bool TryParse(string text, out Currency? currency)
    {
        var currencies = Currency.GetAvailableCurrencies().ToDictionary(c => c.Name);
        
        var str = text.Trim().ToUpperInvariant();
        foreach (var (word, currencyName) in _synonyms)
        {
            if (str == word.ToUpperInvariant())
            {
                currency = currencies[currencyName];
                return true;
            }
        }
        
        if (currencies.TryGetValue(str, out currency))
        {
            return true;
        }

        currency = Currency.GetAvailableCurrencies().FirstOrDefault(c => c.Symbol == str);
        return currency != null;
    }
}