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
            Assert.Contains(Currency.Usd, bank.Rates.Keys);
            Assert.Contains(Currency.Eur, bank.Rates.Keys);
            Assert.Contains(Currency.Rur, bank.Rates.Keys);
        }
    }
}