using System;
using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot;

public interface IRequestParser
{
    bool TryParse(string text, out Money? money, out bool? cash, out Conversion? conversion);
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

    public bool TryParse(string s, out Money? money, out bool? cash, out Conversion? conversion)
    {
        var lowerCaseText = s.ToLowerInvariant();
        cash = ParseCash(lowerCaseText);
        
        var parts = lowerCaseText.Replace("non cash", "").Replace("cash", "").Split(_delimiters, StringSplitOptions.RemoveEmptyEntries);

        money = null;
        conversion = null;
        
        if (parts.Length == 0) return false;

        var convertFrom = Currency.Amd;

        if (_moneyParser.TryParse(parts[0], out money))
        {
            convertFrom = money!.Currency;
        }
        else if (_currencyParser.TryParse(parts[0], out var currency))
        {
            convertFrom = currency!;
        }
        else
        {
            return false;
        }

        if (parts.Length == 1) return true;

        if (_currencyParser.TryParse(parts[1], out var currencyTo))
        {
            conversion = new Conversion { From = convertFrom, To = currencyTo! };
        }
        else if (_moneyParser.TryParse(parts[1], out money))
        {
            conversion = new Conversion { From = convertFrom, To = money!.Currency };
        }

        return true;
    }

    private bool? ParseCash(string s)
    {
        if (s.StartsWith("cash"))
            return true;

        if (s.StartsWith("non cash")) return false;

        return null;
    }
}