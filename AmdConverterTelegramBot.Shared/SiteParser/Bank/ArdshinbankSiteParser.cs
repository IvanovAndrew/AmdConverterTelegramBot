using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class ArdshinbankSiteParser : HtmlTableParserBase
{
    public ArdshinbankSiteParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.ardshinbank.am/en";
    protected override string ExchangeName => "Ardshinbank";

    
    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        string cashId = cash ? "cash" : "no-cash";
        return htmlDocument.DocumentNode.SelectSingleNode($@"//div[@id='{cashId}']/table");
    }
}