using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class ConverseRateParser : HtmlTableParserBase
{
    public ConverseRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.conversebank.am/en/exchange-rate/";

    protected override string ExchangeName => "Converse bank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectNodes(@"//table")[3];
    }

    protected override int BuyIndex(bool cash) => cash ? 3 : 5;
    protected override int SellIndex(bool cash) => cash ? 4 : 6;
}