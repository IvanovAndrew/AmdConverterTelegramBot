using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class HSBCRateParser : HtmlTableParserBase
{
    public HSBCRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.hsbc.am/en-am/help/rates/";
    protected override string ExchangeName => "HSBC Bank Armenia";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($@"//table");
    }

    protected override int BuyIndex(bool cash) => cash ? 3 : 1;
    protected override int SellIndex(bool cash) => cash ? 4 : 2;
}