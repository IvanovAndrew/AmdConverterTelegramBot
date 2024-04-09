using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser;

public abstract class ApiRateParserBase : RateParserBase
{
    protected readonly CurrencyParser _currencyParser;
    protected ApiRateParserBase(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(cultureInfo)
    {
        _currencyParser = currencyParser;
    }
}