using System.Collections.Specialized;
using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.Services;
public static class AmdConverter
{
    public static Result<Dictionary<Bank, decimal>> ConvertAllBanks(List<Bank> banks, Money money, Currency currency, bool toCurrency)
    {
        if (money.Amount <= 0) return Result<Dictionary<Bank, decimal>>.Error("Money is expected to be positive!");
        if (currency != Currency.Amd && money.Currency != Currency.Amd) return Result<Dictionary<Bank, decimal>>.Error("One of the currencies must be AMD");
        
        var nonDramCurrency = currency == Currency.Amd ? money.Currency : currency;
        
        var result = new Dictionary<Bank, decimal>();
        foreach (var bank in banks)
        {
            if (bank.Rates[nonDramCurrency] == Rate.Unknown)
            {
                result[bank] = decimal.MaxValue;
                continue;
            }
                
            result[bank] = AmdConverter.Convert(money, currency, bank.Rates[nonDramCurrency], toCurrency);
        }

        return Result<Dictionary<Bank, decimal>>.Ok(result);
    }
    
    public static decimal Convert(Money money, Currency currency, Rate rate, bool toCurrency)
    {
        if (money.Currency == Currency.Amd)
        {
            return toCurrency? money.Amount / rate.Sell : money.Amount / rate.Buy;
        } 
        else if (currency == Currency.Amd)
        {
            return toCurrency ? money.Amount * rate.Buy : money.Amount * rate.Sell;
        }

        throw new ArgumentException("One of the currencies should be AMD!");
    }
}
