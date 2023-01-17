using System.Globalization;

namespace AmdConverterTelegramBot.SiteParser;

public abstract class SiteParserBase : RateParserBase
{
    
    protected SiteParserBase(CultureInfo cultureInfo) : base(cultureInfo)
    {
    }
}