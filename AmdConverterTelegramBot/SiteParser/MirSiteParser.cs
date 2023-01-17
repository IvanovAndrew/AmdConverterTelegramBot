using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.SiteParser;

public class MirSiteParser : SiteParserBase
{
    private readonly string amdName = "Армянский драм";
    public MirSiteParser() : base(new("ru-RU"))
    {
    }

    protected override string ExchangeName => "MIR";
    public override Result<ExchangePoint> Parse(string source, bool cash)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(source);
        
        if (htmlDocument.DocumentNode == null)
            return Result<ExchangePoint>.Error("Site is unavailable");
        
        return Result<ExchangePoint>.Ok(Parse(htmlDocument, cash));
    }

    protected ExchangePoint Parse(HtmlDocument htmlDocument, bool cash)
    {
        var bank = CreateExchangePoint();
        var table = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='sf-text']");

        foreach (var row in table.SelectNodes("//tr"))
        {
            var nodes = row.SelectNodes("td");
            
            if (nodes == null) continue;
            
            var currencyString = nodes[0].InnerText;

            if (string.Equals(currencyString.Trim(), amdName, StringComparison.InvariantCultureIgnoreCase))
            {
                if (TryParseRate(nodes[1].InnerText, out var rate))
                {
                    bank.AddRate(new Conversion{From = Currency.Rur, To = Currency.Amd}, new Rate(1/rate.FXRate));
                }
            }
        }
        
        return bank;
    }
}