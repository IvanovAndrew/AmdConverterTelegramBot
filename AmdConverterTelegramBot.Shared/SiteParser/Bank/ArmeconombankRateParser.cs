using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class ArmeconombankRateParser : HtmlTableParserBase
{
    public ArmeconombankRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.aeb.am/en/";
    protected override string ExchangeName => "Armeconombank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($@"//table[@id='{(cash? "exchange_table_cash" : "exchange_table_non_cash")}']");
    }
}