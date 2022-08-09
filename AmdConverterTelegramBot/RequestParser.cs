using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot;

public interface IRequestParser
{
    bool TryParse(string text, out Money? money, out Currency? currency, out bool? toCurrency);
}

public class RequestParser : IRequestParser
{
    private readonly string[] _delimiters;
    private readonly IMoneyParser _moneyParser;
    private readonly ICurrencyParser _currencyParser;

    public RequestParser(IMoneyParser moneyParser, ICurrencyParser currencyParser, string[] delimiters)
    {
        _moneyParser = moneyParser;
        _currencyParser = currencyParser;
        _delimiters = delimiters;
    }

    public bool TryParse(string s, out Money? money, out Currency? currency, out bool? toCurrency)
    {
        var parts = s.Split(_delimiters, StringSplitOptions.RemoveEmptyEntries);

        money = null;
        currency = null;
        toCurrency = null;
        
        if (parts.Length == 0) return false;

        if (_moneyParser.TryParse(parts[0], out money))
        {
            toCurrency = true;
        }
        else if (_currencyParser.TryParse(parts[0], out currency))
        {
            toCurrency = false;
        }
        else
        {
            return false;
        }

        if (parts.Length == 1) return true;

        if (toCurrency == true)
        {
            _currencyParser.TryParse(parts[1], out currency);
        }
        else if (toCurrency == false)
        {
            _moneyParser.TryParse(parts[1], out money);
        }

        return true;
    }
}