using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class ByblosRateParser : HtmlTableParserBase
{
    public ByblosRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.byblosbankarmenia.am/en";

    protected override string ExchangeName => "Byblos bank Armenia";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectNodes($@"//table[@class='currency_table fluid-x']")[cash ? 0 : 1];
    }
}