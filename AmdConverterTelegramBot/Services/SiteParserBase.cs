using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services;

public abstract class SiteParserBase
{
    private readonly CultureInfo _cultureInfo;
    protected SiteParserBase(CultureInfo cultureInfo)
    {
        _cultureInfo = cultureInfo;
    }
    
    public Result<ExchangePoint> Parse(string html)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        
        if (htmlDocument.DocumentNode == null)
            return Result<ExchangePoint>.Error("Site is unavailable");
        
        return Result<ExchangePoint>.Ok(Parse(htmlDocument));
    }
    
    protected decimal ParseRate(string s) => decimal.Parse(s, _cultureInfo);

    public abstract ExchangePoint Parse(HtmlDocument htmlDocument);
}