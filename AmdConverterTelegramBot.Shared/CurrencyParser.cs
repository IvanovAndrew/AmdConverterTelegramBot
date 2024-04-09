using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared.Entities;

namespace AmdConverterTelegramBot.Shared;

internal static class RequestDictionary
{
    internal static IReadOnlyDictionary<string, Currency> CurrencySynonyms = new Dictionary<string, Currency>()
    {
        ["amd"] = Currency.Amd,
        ["dram"] = Currency.Amd,
        ["драм"] = Currency.Amd,
        ["драмов"] = Currency.Amd,
        ["драма"] = Currency.Amd,
        ["армянских драм"] = Currency.Amd,
        ["армянский драм"] = Currency.Amd,
        ["армянских драма"] = Currency.Amd,
        ["армянских драмов"] = Currency.Amd,
        ["лари"] = Currency.GEL,
        ["грузинский лари"] = Currency.GEL,
        ["грузинских лари"] = Currency.GEL,
        ["lari"] = Currency.GEL,
        ["usd"] = Currency.Usd,
        ["dollar"] = Currency.Usd,
        ["dollars"] = Currency.Usd,
        ["доллар"] = Currency.Usd,
        ["долларов"] = Currency.Usd,
        ["долларах"] = Currency.Usd,
        ["доллары"] = Currency.Usd,
        ["eur"] = Currency.Eur,
        ["euro"] = Currency.Eur,
        ["евро"] = Currency.Eur,
        ["rur"] = Currency.Rur,
        ["rub"] = Currency.Rur,
        ["ruble"] = Currency.Rur,
        ["rubles"] = Currency.Rur,
        ["рубль"] = Currency.Rur,
        ["рубли"] = Currency.Rur,
        ["рублях"] = Currency.Rur,
        ["рублей"] = Currency.Rur
    };
    
    internal static string[] Delimiters = {" в ", "->", " in ", " to ", " as "};
}

public class CurrencyParser
{
    public CurrencyParser()
    {
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
        
        if (RequestDictionary.CurrencySynonyms.TryGetValue(str.ToLowerInvariant(), out currency))
        {
            return true;
        }
        
        currency ??= Currency.Amd;
        
        return false;
    }

    public Currency Parse(string text)
    {
        var availableCurrencies = Currency.GetAvailableCurrencies();
        var currencies = availableCurrencies.ToDictionary(c => c.Name);
        
        var str = text.Trim().ToUpperInvariant();
        
        if (currencies.TryGetValue(str, out var currency))
        {
            return currency;
        }
        
        currency = availableCurrencies.FirstOrDefault(c => string.Equals(c.Symbol, str, StringComparison.InvariantCultureIgnoreCase));

        if (currency != null)
        {
            return currency;
        }

        if (RequestDictionary.CurrencySynonyms.TryGetValue(str.ToLowerInvariant(), out currency))
        {
            return currency;
        }
        
        return Currency.Amd;
    }
    
    
}