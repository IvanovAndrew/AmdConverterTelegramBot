using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser;

public abstract class HtmlTableParserBase : HtmlParserBase
{
    protected HtmlTableParserBase(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode tableNode) => tableNode.SelectNodes("tbody/tr");
    protected override string ColumnSeparator() => "td";
}