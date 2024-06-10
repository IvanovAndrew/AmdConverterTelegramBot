using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared.Entities;

namespace AmdConverterTelegramBot.Shared.SiteParser;

public abstract class RateParserBase
{
    protected readonly CultureInfo CultureInfo;
    internal abstract string Url { get; }
    protected virtual bool CardOperationAllowed { get; } = true;
    protected abstract string ExchangeName { get; }

    protected RateParserBase(CultureInfo cultureInfo)
    {
        CultureInfo = cultureInfo;
    }

    public abstract Result<ExchangePoint> Parse(string source, bool cash);

    protected ExchangePoint CreateExchangePoint(Currency baseCurrency = null) =>
        new() { Name = ExchangeName, BaseCurrency = baseCurrency ?? Currency.Amd };
}