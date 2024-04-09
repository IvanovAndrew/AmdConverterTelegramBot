using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class IdbankRateParser : HtmlParserBase
{
    public IdbankRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://idbank.am/en/rates/";
    protected override string ExchangeName => "IDbank";
    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode("//div[@class='m-exchange__table']");
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode node)
    {
        return node.SelectNodes("div[@class='m-exchange__table-row']");
    }

    protected override string ColumnSeparator()
    {
        return "div";
    }

    protected override string ExtractCurrency(HtmlNode rate)
    {
        return rate.InnerText.Replace("1", "").Trim();
    }
}