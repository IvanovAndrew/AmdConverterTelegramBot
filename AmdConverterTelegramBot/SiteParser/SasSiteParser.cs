using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser;

public class SasSiteParser : HtmlParserBase
{
    public SasSiteParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "SAS";
    protected override bool NonCashOperationAllowed => false;

    public override Result<ExchangePoint> Parse(string source, bool cash)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(source);
        
        if (!cash && !NonCashOperationAllowed)
            return Result<ExchangePoint>.Error("Non cash exchange isn't allowed");
        
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
