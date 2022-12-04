using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using Xunit;

namespace AmdConverterTelegramBotTests
{
    public class SasParserTest
    {
        [Fact]
        public async Task Parse()
        {
            // Act
            var bank =
                await new SasSiteParser(new CurrencyParser(new Dictionary<string, string>()), CultureInfo.InvariantCulture).ParseAsync(
                    "https://www.sas.am/food/en/");
            
            // Assert
            Assert.Equal("SAS", bank.Name);
            Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Usd}, bank.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Eur}, bank.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Rur}, bank.Rates.Keys);
            
            Assert.Contains(new Conversion(){From = Currency.Usd, To = Currency.Amd}, bank.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Eur, To = Currency.Amd}, bank.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Rur, To = Currency.Amd}, bank.Rates.Keys);
        }
    }
}