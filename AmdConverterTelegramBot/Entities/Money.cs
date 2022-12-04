using System.Globalization;
using System.Text.RegularExpressions;

namespace AmdConverterTelegramBot.Entities
{
    public record Money
    {
        public Currency Currency { get; init; }
        public decimal Amount { get; init; }
    
        public override string ToString() => $"{Amount}{Currency.Symbol}";
    }
    
    public interface IMoneyParser
    {
        bool TryParse(string text, out Money? money);
    }
    
    public class MoneyParser : IMoneyParser
    {
        private readonly ICurrencyParser _currencyParser;
        private string? _defaultCurrency;
    
        public MoneyParser(ICurrencyParser currencyParser, string? defaultCurrency)
        {
            _currencyParser = currencyParser;
            _defaultCurrency = defaultCurrency;
        }
        
        public bool TryParse(string? text, out Money money)
        {
            money = new Money();
            
            Regex amountRegex = new Regex(@"((\d+\s?)+\.?\d*)");
    
            var str = text.Replace(",", ".");
            Match match = amountRegex.Match(str);
            
            if (!match.Success) return false;
    
            var numberPart = match.Groups[0].Value;
    
            if (!_currencyParser.TryParse(str.Replace(numberPart, ""), out var currency))
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
}

