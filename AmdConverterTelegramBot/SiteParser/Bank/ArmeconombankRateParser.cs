using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

class ArmeconombankRateParser : HtmlTableParserBase
{
    public ArmeconombankRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Armeconombank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($@"//table[@id='{(cash? "exchange_table_cash" : "exchange_table_non_cash")}']");
    }
}