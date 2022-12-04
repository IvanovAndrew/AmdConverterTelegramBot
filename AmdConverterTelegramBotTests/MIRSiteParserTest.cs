using System.Threading.Tasks;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using Xunit;

namespace AmdConverterTelegramBotTests;

public class MIRSiteParserTest
{
    [Fact]
    public async Task Parse()
    {
        // Act
        var exchangePoint =
            await new MirSiteParser().ParseAsync(
                "https://mironline.ru/support/list/kursy_mir/");
            
        // Assert
        Assert.Equal("MIR", exchangePoint.Name);
        Assert.Equal(Currency.Rur, exchangePoint.BaseCurrency);
        Assert.Contains(new Conversion(){From = Currency.Rur, To = Currency.Amd}, exchangePoint.Rates.Keys);
        Assert.DoesNotContain(new Conversion(){From = Currency.Amd, To = Currency.Rur}, exchangePoint.Rates.Keys);
    }
}