using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services;

public class MirSiteParser : SiteParserBase
{
    private readonly string amdName = "Армянский драм";
    public MirSiteParser() : base(new("ru-RU"))
    {
    }
    
    public override ExchangePoint Parse(HtmlDocument htmlDocument)
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
                var rate = ParseRate(nodes[1].InnerText);
                
                bank.AddRate(new Conversion{From = Currency.Rur, To = Currency.Amd}, new Rate(1/rate));
            }
        }
        
        return bank;
    }
}