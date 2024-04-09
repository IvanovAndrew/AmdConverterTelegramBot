using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared.Entities;

namespace AmdConverterTelegramBot.Shared.SiteParser;

public abstract class RateParserBase
{
    protected readonly CultureInfo _cultureInfo;
    internal abstract string Url { get; }
    protected virtual bool CardOperationAllowed { get; } = true;
    protected abstract string ExchangeName { get; }

    protected RateParserBase(CultureInfo cultureInfo)
    {
        _cultureInfo = cultureInfo;
    }

    public abstract Result<ExchangePoint> Parse(string source, bool cash);

    protected ExchangePoint CreateExchangePoint(Currency baseCurrency = null) =>
        new() { Name = ExchangeName, BaseCurrency = baseCurrency ?? Currency.Amd };

    protected bool TryParseRate(string s, out Rate rate)
    {
        rate = Rate.Unknown;
        if (decimal.TryParse(s, NumberStyles.Any, _cultureInfo, out decimal value) && value > 0)
        {
            rate = new Rate(value);
            return true;
        }

        return false;
    }
}