using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services;

public class SasSiteParser : SiteParserBase
{
    private readonly ICurrencyParser _currencyParser;
    public SasSiteParser(ICurrencyParser currencyParser, CultureInfo cultureInfo) : base(cultureInfo)
    {
        _currencyParser = currencyParser?? throw new ArgumentNullException(nameof(currencyParser));
    }

    public override ExchangePoint Parse(HtmlDocument htmlDocument)
    {
        var exchangePoint = new ExchangePoint(){Name = "SAS", BaseCurrency = Currency.Amd};
        var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='exchange-table']");

        foreach (var row in table.SelectNodes("//div[@class='exchange-table__row']"))
        {
            var nodes = row.SelectNodes("div");
            if (_currencyParser.TryParse(nodes[0].SelectSingleNode("span").InnerText, out var currency))
            {
                var buy = ParseRate(nodes[1].SelectSingleNode("span").InnerText);
                var sell= ParseRate(nodes[2].SelectSingleNode("span").InnerText);
                
                exchangePoint.AddRate(new Conversion {From = Currency.Amd, To = currency!}, new Rate(sell));
                exchangePoint.AddRate(new Conversion {From = currency!, To = Currency.Amd}, new Rate(buy));
            }
            
        }
        
        return exchangePoint;
    }
}
