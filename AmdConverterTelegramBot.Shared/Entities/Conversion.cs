using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.Shared.Entities;

public record Conversion
{
    public Currency From { get; init; }
    public Currency To { get; init; }

    public override string ToString()
    {
        return $"{From.Symbol} -> {To.Symbol}";
    }
}

public abstract class MoneyConversion
{
    public Currency From { get; protected init; }
    public Currency To { get; protected init; }
    public Money Money { get; protected init; }

    public override string ToString()
    {
        return $"{From.Symbol} -> {To.Symbol}";
    }
}

public class MoneyToCurrencyConversion : MoneyConversion
{
    public MoneyToCurrencyConversion(Money money, Currency targetCurrency)
    {
        Money = money;
        From = money.Currency;
        To = targetCurrency;
    }
}

public class CurrencyToMoneyConversion : MoneyConversion
{
    public CurrencyToMoneyConversion(Currency sourceCurrency, Money money)
    {
        Money = money;
        From = sourceCurrency;
        To = money.Currency;
    }
}