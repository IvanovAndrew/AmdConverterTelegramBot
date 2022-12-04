using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services;

public class MirSiteParser
{
    private readonly CultureInfo _cultureInfo = new CultureInfo("ru-ru");
    private readonly string amdName = "Армянский драм";
    public MirSiteParser()
    {
    }

    public async Task<ExchangePoint> ParseAsync(string url)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
        
        var htmlDocument = await new HtmlWeb().LoadFromWebAsync(url);

        return Parse(htmlDocument);
    }
    
    private ExchangePoint Parse(HtmlDocument htmlDocument)
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