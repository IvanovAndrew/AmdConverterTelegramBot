using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser;

abstract class HtmlTableParserBase : HtmlParserBase
{
    protected HtmlTableParserBase(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode tableNode) => tableNode.SelectNodes("tbody/tr");
    protected override string ColumnSeparator() => "td";
}