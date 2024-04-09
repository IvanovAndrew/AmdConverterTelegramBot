using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared.Entities;
using Sprache;

namespace AmdConverterTelegramBot.Shared;

// grammar:= money | way money | way ((money to CURRENCY) | (CURRENCY to money)) 
// way: CASH | CARD
// money: \d+ CURRENCY
// to: -> | TO

public static class Grammar
{
    private static readonly CurrencyParser _currencyParser = new();

    private static readonly Parser<decimal> Amount =
        from integerParts in Parse.Number.DelimitedBy(Parse.Char(' '))
        select decimal.Parse(string.Concat(integerParts));

    private static readonly Parser<Currency> CurrencyParser =
        Parse.Letter.AtLeastOnce().Text().Select(s => _currencyParser.Parse(s))
        .Or
        (
            Parse.Chars(Currency.GetAvailableSymbols()).Select(s => _currencyParser.Parse(s.ToString()))
        );

    private static readonly Parser<Money> Money =
        from amount in Amount
        from space in Parse.WhiteSpace.Optional()
        from currency in CurrencyParser
        select new Money {Amount = amount, Currency = currency};

    private static readonly Parser<bool> Cash =
        Parse.IgnoreCase("cash").Return(true);

    private static readonly Parser<bool> Card =
        Parse.IgnoreCase("card").Return(false);
    
    private static readonly Parser<bool> Way = Cash.Or(Card);

    private static readonly Parser<bool> Delimiter =
        from leftSpace in Parse.WhiteSpace.Optional()
        from value in Parse.String("->").Or(Parse.String("to")).Or(Parse.String("в")).Or(Parse.String("in"))
        from rightSpace in Parse.WhiteSpace.Optional()
        select false;

    private static readonly Parser<MoneyConversion> Conversion =
            (
                from money in Money
                from leftSpace in Parse.WhiteSpace.Optional()
                from arrow in Delimiter
                from rightSpace in Parse.WhiteSpace.Optional()
                from currency in CurrencyParser
                select new MoneyToCurrencyConversion(money, currency)
            ).Or
            (
                from currency in CurrencyParser
                from leftSpace in Parse.WhiteSpace.Optional()
                from arrow in Delimiter
                from rightSpace in Parse.WhiteSpace.Optional()
                from money in Money
                select (MoneyConversion)new CurrencyToMoneyConversion(currency, money))
        // ).Or
        // (
        //     from money in Money
        //     from leftSpace in Parse.WhiteSpace.Optional()
        //     from arrow in Delimiter
        //     from rightSpace in Parse.WhiteSpace.Optional()
        //     from currency in Currency
        //     select new MoneyToCurrencyConversion(money, currency)
        // );
        ;

    private static Parser<UserRequest> AmountOnly =
        from amount in Amount
        select new AmountState() { Amount = amount };
    
    private static Parser<UserRequest> MoneyOnly =
        from money in Money
        select new MoneyState() { Money = money };
    
    private static Parser<UserRequest> MoneyAndWay =
        from way in Way
        from space in Parse.WhiteSpace.Optional()
        from money in Money
        select new MoneyAndWayState() { Money = money, Cash = way};
    
    private static Parser<UserRequest> FullRequest = 
        from way in Way
        from space in Parse.WhiteSpace
        from conversion in Conversion 
        select new FullRequest()
        {
            Cash = way,
            Conversion = conversion
        };

    public static Parser<UserRequest> Request =
        FullRequest.Or(MoneyAndWay).Or(MoneyOnly).Or(AmountOnly);
}

public abstract class UserRequest
{
    
}

public class AmountState : UserRequest
{
    public decimal Amount { get; init; }
}

public class MoneyState : UserRequest
{
    public Money Money { get; init; }
}

public class MoneyAndWayState : UserRequest
{
    public Money Money { get; init; }
    public bool Cash { get; init; }
}

public class FullRequest : UserRequest
{
    public bool Cash { get; set; }
    public MoneyConversion Conversion { get; set; }
}