using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

class ArdshinbankSiteParser : HtmlTableParserBase
{
    public ArdshinbankSiteParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Ardshinbank";

    
    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        string cashId = cash ? "cash" : "no-cash";
        return htmlDocument.DocumentNode.SelectSingleNode($@"//div[@id='{cashId}']/table");
    }
}