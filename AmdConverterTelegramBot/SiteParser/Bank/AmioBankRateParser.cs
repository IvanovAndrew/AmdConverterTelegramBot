using System.Globalization;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser.Bank;

public class AmioBankRateParser : HtmlTableParserBase
{
    public AmioBankRateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    protected override string ExchangeName => "Amiobank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($@"//table[@class='chakra-table css-5605sr']");
    }
}