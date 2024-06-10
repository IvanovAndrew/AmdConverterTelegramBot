using System.Globalization;
using AmdConverterTelegramBot.Shared.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser;

public class BankRateLoader
{
    private readonly RateParserBase _parser;
    public BankRateLoader(RateParserBase parser)
    {
        _parser = parser;
    }
    
    internal async Task<Result<ExchangePoint>> GetExchangePointAsync(HttpClient httpClient, bool cash)
    {
        try
        {
            var source = await httpClient.GetStringAsync(_parser.Url);
            return _parser.Parse(source, cash);
        }
        catch(Exception e)
        {
            return Result<ExchangePoint>.Error($"Couldn't parse {_parser.Url}");
        }
    }
}

public class SasSiteParser : HtmlParserBase
{
    public SasSiteParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.sas.am/food/en/";

    protected override string ExchangeName => "SAS";
    protected override bool CardOperationAllowed => false;

    public override Result<ExchangePoint> Parse(string source, bool cash)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(source);
        
        if (htmlDocument.DocumentNode == null)
            return Result<ExchangePoint>.Error("Site is unavailable");
        
        return Result<ExchangePoint>.Ok(Parse(htmlDocument, cash));
    }

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode("//div[@class='exchange-table']");
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode node)
    {
        return node.SelectNodes("//div[@class='exchange-table__row']");
    }

    protected override string ColumnSeparator()
    {
        return "div";
    }
}
