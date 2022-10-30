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

    public async Task<Bank> ParseAsync(string url)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
        
        var htmlDocument = await new HtmlWeb().LoadFromWebAsync(url);

        return Parse(htmlDocument);
    }
    
    private Bank Parse(HtmlDocument htmlDocument)
    {
        var bank = new Bank(){Name = "SAS"};
        var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='exchange-table']");

        foreach (var row in table.SelectNodes("//div[@class='exchange-table__row']"))
        {
            var nodes = row.SelectNodes("div");
            if (_currencyParser.TryParse(nodes[0].SelectSingleNode("span").InnerText, out var currency))
            {
                var buy = ParseRate(nodes[1].SelectSingleNode("span").InnerText);
                var sell= ParseRate(nodes[2].SelectSingleNode("span").InnerText);
                
                bank.AddRate(currency!, new Rate(buy, sell));
            }
            
        }
        
        return bank;
    }

    private decimal ParseRate(string s) => decimal.Parse(s, _cultureInfo);
}
