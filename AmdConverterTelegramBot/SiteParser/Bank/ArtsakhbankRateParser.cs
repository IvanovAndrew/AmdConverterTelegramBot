using System.Globalization;
using AmdConverterTelegramBot.Services;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

class ArtsakhbankRateParser : HtmlParserBase
{
    public ArtsakhbankRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Artsakhbank";
    

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        string cashId = cash ? "tab_block exchange_1" : "tab_block exchange_2";
        return htmlDocument.DocumentNode.SelectSingleNode($@"//div[@class='{cashId}']")?? htmlDocument.DocumentNode.SelectSingleNode($@"//div[@class='{cashId} selected']");
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode tableNode)
    {
        return tableNode.SelectNodes("ul[@class='exchange_list']/li/ul");
    }

    protected override string ColumnSeparator()
    {
        return "li";
    }
}