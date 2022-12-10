using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services;

public class SasSiteParser
{
    private readonly ICurrencyParser _currencyParser;
    private readonly CultureInfo _cultureInfo;
    public SasSiteParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        _currencyParser = currencyParser?? throw new ArgumentNullException(nameof(currencyParser));
        _cultureInfo = cultureInfo;
    }

    public ExchangePoint Parse(string html)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);

        return Parse(htmlDocument);
    }
    public ExchangePoint Parse(HtmlDocument htmlDocument)
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

    private decimal ParseRate(string s) => decimal.Parse(s, _cultureInfo);
}
