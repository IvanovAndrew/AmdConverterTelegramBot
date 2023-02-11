using System;
using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot;

public interface IRequestParser
{
    bool TryParseAmount(string? text, out decimal amount);
    bool TryParseMoney(string? text, out Money money);
    bool TryParseMoneyAndCash(string? text, out Money money, out bool cash);
    bool TryParseFullRequest(string? text, out Money money, out bool cash, out Conversion conversion);
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

    public bool TryParseAmount(string? text, out decimal amount)
    {
        return decimal.TryParse(text, out amount);
    }
    
    public bool TryParseMoney(string? text, out Money money)
    {
        return _moneyParser.TryParse(text?? string.Empty, out money);
    }

    public bool TryParseMoneyAndCash(string? text, out Money money, out bool cash)
    {
        money = new Money();
        
        var lowerCaseText = (text?? "").ToLowerInvariant();
        if (TryParseCash(lowerCaseText, out cash))
        {
            var parts = lowerCaseText.Replace("non cash", "").Replace("cash", "").Split(_delimiters, StringSplitOptions.None);

            if (_moneyParser.TryParse(parts[0], out money))
            {
                return true;
            }
            else if (parts.Length > 1 && _moneyParser.TryParse(parts[1], out money))
            {
                return true;
            }
        }

        return false;
    }

    public bool TryParseFullRequest(string? s, out Money money, out bool cash, out Conversion conversion)
    {
        conversion = new Conversion();
        
        if (!TryParseMoneyAndCash(s, out money, out cash))
        {
            return false;
        }
        
        var parts = (s?? String.Empty).Replace("non cash", "").Replace("cash", "").Split(_delimiters, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2) return false;

        if (_currencyParser.TryParse(parts[0], out var currencyFrom))
        {
            conversion = new Conversion { From = currencyFrom, To = money.Currency };
        }
        else if (_currencyParser.TryParse(parts[1], out var currencyTo))
        {
            conversion = new Conversion { From = money.Currency, To = currencyTo };
        }
        else
        {
            return false;
        }
        
        return true;
    }

    private bool TryParseCash(string s, out bool cash)
    {
        cash = false;

        if (s.StartsWith("cash"))
        {
            cash = true;
            return true;
        }

        if (s.StartsWith("non cash"))
        {
            cash = false;
            return true;
        }

        return false;
    }
}