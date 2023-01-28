using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

class ConverseRateParser : HtmlTableParserBase
{
    public ConverseRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Converse bank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectNodes(@"//table")[3];
    }

    protected override int BuyIndex(bool cash) => cash ? 3 : 5;
    protected override int SellIndex(bool cash) => cash ? 4 : 6;
}