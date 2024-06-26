using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class AmeriabankRatesParser : HtmlTableParserBase
{
    public AmeriabankRatesParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {}

    internal override string Url => "https://ameriabank.am/en";
    protected override string ExchangeName => "Ameriabank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($@"//table[@id='dnn_ctr20025_View_grdRates']");
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode tableNode) => tableNode.SelectNodes("tr");

    protected override int BuyIndex(bool cash) => cash ? 1 : 3;
    protected override int SellIndex(bool cash) => cash ? 2 : 4;
}