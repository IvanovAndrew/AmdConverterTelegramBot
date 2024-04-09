using System.Globalization;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Shared.SiteParser.Bank;

class UnibankRateParser : HtmlParserBase
{
    public UnibankRateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) : base(currencyParser, cultureInfo)
    {
    }

    internal override string Url => "https://www.unibank.am/en/";
    protected override string ExchangeName => "Unibank";
    protected override HtmlNode SelectTableNode(HtmlDocument htmlDocument, bool cash)
    {
        return htmlDocument.DocumentNode.SelectSingleNode($"//div[@id='{(cash ? "Cash" : "Noncash")}']");
    }

    protected override HtmlNodeCollection SelectTableRows(HtmlNode node)
    {
        var collection = node.SelectNodes("div/ul[2]/li")?? node.SelectNodes("div/ul/li");

        var newCollection = new HtmlNodeCollection(node);

        int index = 0; 
        while (index < collection.Count)
        {
            var newNode = new HtmlNode(HtmlNodeType.Element, node.OwnerDocument, index/3);
            newNode.AppendChild(collection[index++]);
            newNode.AppendChild(collection[index++]);
            newNode.AppendChild(collection[index++]);
        
            newCollection.Add(newNode);
        }
        

        return newCollection;
    }

    protected override string ColumnSeparator()
    {
        return "li";
    }
}