using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

class AraratbankRateParser : HtmlTableParserBase
{
    public AraratbankRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Araratbank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        int cashIndex = cash ? 0 : 1;
        return htmlDocument.DocumentNode.SelectNodes($@"//table[@class='exchange__table']")[cashIndex];
    }
}