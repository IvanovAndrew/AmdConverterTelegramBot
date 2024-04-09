using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class AcbaSiteParser : HtmlParserBase
{
    public AcbaSiteParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.acba.am/en/";
    protected override string ExchangeName => "Acba bank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectNodes(
                $@"//div[@class='simple_price__bodys__item']")[cash ? 0 : 1];
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode tableNode)
    {
        return tableNode.SelectNodes("div[@class='simple_price-row']");
    }

    protected override string ColumnSeparator()
    {
        return "div";
    }
}