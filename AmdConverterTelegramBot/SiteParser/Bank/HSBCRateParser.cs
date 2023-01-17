using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

class HSBCRateParser : HtmlTableParserBase
{
    public HSBCRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "HSBC Bank Armenia";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($@"//table");
    }

    protected override int BuyIndex(bool cash) => cash ? 3 : 1;
    protected override int SellIndex(bool cash) => cash ? 4 : 2;
}