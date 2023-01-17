using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

class AcbaSiteParser : HtmlParserBase
{
    public AcbaSiteParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

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