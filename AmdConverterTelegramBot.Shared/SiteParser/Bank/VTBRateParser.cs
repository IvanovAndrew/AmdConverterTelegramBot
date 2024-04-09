using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class VTBRateParser : HtmlTableParserBase
{
    public VTBRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.vtb.am/ru/currency";
    protected override string ExchangeName => "VTB Bank (Armenia)";
    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectNodes("//table[@class='exchange-rate-table_no-width']")[cash ? 0 : 1];
    }

    protected override string ExtractCurrency(HtmlNode rate) => rate.SelectSingleNode("div/span").InnerText;
}