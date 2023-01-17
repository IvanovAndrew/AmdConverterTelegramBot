using System.Globalization;

namespace AmdConverterTelegramBot.SiteParser;

public abstract class ApiRateParserBase : RateParserBase
{
    protected readonly ICurrencyParser _currencyParser;
    protected ApiRateParserBase(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(cultureInfo)
    {
        _currencyParser = currencyParser;
    }
}