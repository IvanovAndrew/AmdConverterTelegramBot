using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services;

public class MirSiteParser
{
    private readonly CultureInfo _cultureInfo = new("ru-RU");
    private readonly string amdName = "Армянский драм";
    public MirSiteParser()
    {
    }

    public ExchangePoint Parse(string html)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(html);
        return Parse(htmlDocument);
    }
    
    public ExchangePoint Parse(HtmlDocument htmlDocument)
    {
        var bank = new ExchangePoint(){Name = "MIR", BaseCurrency = Currency.Amd};
        var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='sf-text']");

        foreach (var row in table.SelectNodes("//tr"))
        {
            var nodes = row.SelectNodes("td");
            
            if (nodes == null) continue;
            
            var currencyString = nodes[0].InnerText;

            if (string.Equals(currencyString.Trim(), amdName, StringComparison.InvariantCultureIgnoreCase))
            {
                var rate = decimal.Parse(nodes[1].InnerText, _cultureInfo);
                
                bank.AddRate(new Conversion{From = Currency.Rur, To = Currency.Amd}, new Rate(1/rate));
            }
        }
        
        return bank;
    }

    private decimal ParseRate(string s) => decimal.Parse(s, _cultureInfo);
}