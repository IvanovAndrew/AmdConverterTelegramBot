using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class AraratbankRateParser : HtmlTableParserBase
{
    public AraratbankRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.araratbank.am/en/";
    protected override string ExchangeName => "Araratbank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        int cashIndex = cash ? 0 : 1;
        return htmlDocument.DocumentNode.SelectNodes($@"//table[@class='exchange__table']")[cashIndex];
    }
}