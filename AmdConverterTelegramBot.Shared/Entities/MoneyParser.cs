using System.Globalization;
using System.Text.RegularExpressions;
using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.Shared.Entities;

public class MoneyParser : IMoneyParser
{
    private readonly CurrencyParser _currencyParser;

    public MoneyParser(CurrencyParser currencyParser)
    {
        _currencyParser = currencyParser;
    }

    public bool TryParse(string? text, out Money money)
    {
        money = new Money();

        if (text == null) return false;

        Regex amountRegex = new Regex(@"((\d+\s?)+\.?\d*)");

        var str = text.Replace(",", ".");
        Match match = amountRegex.Match(str);

        if (!match.Success) return false;

        var numberPart = match.Groups[0].Value;
        var position = text.IndexOf(numberPart);

        if (!_currencyParser.TryParse(str.Replace(numberPart, ""), out var currency))
        {
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