using System.Threading.Tasks;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using HtmlAgilityPack;
using Xunit;

namespace AmdConverterTelegramBotTests;

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
        var exchangePoint = new MirSiteParser().Parse(GetHtmlDocument());
            
        // Assert
        Assert.Equal("MIR", exchangePoint.Name);
        Assert.Equal(Currency.Amd, exchangePoint.BaseCurrency);
        Assert.Contains(new Conversion(){From = Currency.Rur, To = Currency.Amd}, exchangePoint.Rates.Keys);
        Assert.DoesNotContain(new Conversion(){From = Currency.Amd, To = Currency.Rur}, exchangePoint.Rates.Keys);
    }
}