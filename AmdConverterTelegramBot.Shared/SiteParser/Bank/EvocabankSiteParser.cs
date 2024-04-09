using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class EvocabankSiteParser : HtmlTableParserBase
{
    public EvocabankSiteParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }


    internal override string Url => "https://www.evoca.am/en";
    protected override string ExchangeName => "Evocabank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectNodes($@"//table[@class='exchange__table']")[cash ? 0 : 1];
    }
}