using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

public class AmioBankRateParser : HtmlTableParserBase
{
    public AmioBankRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.amiobank.am/en";
    protected override string ExchangeName => "Amiobank";

    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($@"//table[@class='chakra-table css-5605sr']");
    }
}