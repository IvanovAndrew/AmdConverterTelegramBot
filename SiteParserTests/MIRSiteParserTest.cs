using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.SiteParser;
using HtmlAgilityPack;
using Xunit;

namespace SiteParsersTests;

public class MIRSiteParserTest
{
    private HtmlDocument GetHtmlDocument()
    {
        var webDocument = new HtmlWeb();
        return webDocument.Load("https://mironline.ru/support/list/kursy_mir/");
    }
    
    [Fact]
    public void Parse()
    {
        // Act
        var exchangePoint = new MirSiteParser().Parse(GetHtmlDocument().ParsedText, true);
            
        // Assert
        Assert.True(exchangePoint.IsSuccess);
        Assert.Equal("MIR", exchangePoint.Value.Name);
        Assert.Equal(Currency.Amd, exchangePoint.Value.BaseCurrency);
        Assert.Contains(new Conversion(){From = Currency.Rur, To = Currency.Amd}, exchangePoint.Value.Rates.Keys);
        Assert.DoesNotContain(new Conversion(){From = Currency.Amd, To = Currency.Rur}, exchangePoint.Value.Rates.Keys);
    }
}