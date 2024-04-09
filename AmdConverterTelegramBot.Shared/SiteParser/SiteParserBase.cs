using System.Globalization;

namespace AmdConverterTelegramBot.Shared.SiteParser;

public abstract class SiteParserBase : RateParserBase
{
    protected SiteParserBase(CultureInfo cultureInfo) : base(cultureInfo)
    {
    }
}