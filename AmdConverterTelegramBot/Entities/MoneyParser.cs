using System.Globalization;
using System.Text.RegularExpressions;

namespace AmdConverterTelegramBot.Entities;

public class MoneyParser : IMoneyParser
{
    private readonly ICurrencyParser _currencyParser;
    private readonly string? _defaultCurrency;
    
    public MoneyParser(ICurrencyParser currencyParser, string? defaultCurrency)
    {
        _currencyParser = currencyParser;
        _defaultCurrency = defaultCurrency;
    }
        
    public bool TryParse(string text, out Money money)
    {
        money = new Money();
            
        Regex amountRegex = new Regex(@"((\d+\s?)+\.?\d*)");
    
        var str = text.Replace(",", ".");
        Match match = amountRegex.Match(str);
            
        if (!match.Success) return false;
    
        var numberPart = match.Groups[0].Value;
        var position = text.IndexOf(numberPart);
    
        if (!_currencyParser.TryParse(str.Substring(position).Replace(numberPart, ""), out var currency))
        {
            if (!_currencyParser.TryParse(_defaultCurrency, out currency))
                return false;
        }
    
        NumberFormatInfo numberFormatInfo = new NumberFormatInfo
        {
            CurrencyGroupSeparator = ".",
            CurrencySymbol = currency!.Symbol,
        };
    
        if (decimal.TryParse(numberPart.Replace(" ", ""), NumberStyles.Currency, numberFormatInfo, out var amount))
        {
            money = new Money { Amount = amount, Currency = currency };
            return true;
        }
    
        return false;
    }
}