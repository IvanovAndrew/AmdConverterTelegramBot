using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser;

public abstract class HtmlParserBase : SiteParserBase
{
    private readonly ICurrencyParser _currencyParser;
    protected HtmlParserBase(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(cultureInfo)
    {
        _currencyParser = currencyParser;
    }
    
    public override Result<ExchangePoint> Parse(string html, bool cash)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        
        if (htmlDocument.DocumentNode == null)
            return Result<ExchangePoint>.Error("Site is unavailable");
        
        return Result<ExchangePoint>.Ok(Parse(htmlDocument, cash));
    }

    protected ExchangePoint Parse(HtmlDocument htmlDocument, bool cash)
    {
        var exchangePoint = CreateExchangePoint();
        var tableNode = SelectTableNode(htmlDocument, cash);

        var buyIndex = BuyIndex(cash);
        var sellIndex = SellIndex(cash);
        foreach (var row in SelectTableRows(tableNode))
        {
            var nodes = row.SelectNodes(ColumnSeparator());
            if (nodes == null) continue;
            
            if (_currencyParser.TryParse(ExtractCurrency(nodes[CurrencyIndex()]), out var currency))
            {
                if (TryParseRate(nodes[buyIndex].InnerText, out var buy))
                {
                    exchangePoint.AddRate(new Conversion {From = currency!, To = Currency.Amd}, buy);
                }

                if (TryParseRate(nodes[sellIndex].InnerText, out var sell))
                {
                    exchangePoint.AddRate(new Conversion {From = Currency.Amd, To = currency!}, sell);
                }
            }
        }
        
        return exchangePoint;
    }

    protected abstract HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash);
    protected abstract HtmlNodeCollection SelectTableRows(HtmlNode node);
    protected abstract string ColumnSeparator();
    
    protected virtual string ExtractCurrency(HtmlNode rate) => rate.InnerText;
    protected virtual int CurrencyIndex() => 0;
    protected virtual int BuyIndex(bool cash) => 1;
    protected virtual int SellIndex(bool cash) => 2;
}