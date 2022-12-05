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
            var exchangePoint = await Execute();
            
            // Assert
            Assert.Equal("SAS", exchangePoint.Name);
            Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Usd}, exchangePoint.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Eur}, exchangePoint.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Amd, To = Currency.Rur}, exchangePoint.Rates.Keys);
            
            Assert.Contains(new Conversion(){From = Currency.Usd, To = Currency.Amd}, exchangePoint.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Eur, To = Currency.Amd}, exchangePoint.Rates.Keys);
            Assert.Contains(new Conversion(){From = Currency.Rur, To = Currency.Amd}, exchangePoint.Rates.Keys);
        }

        
        [Fact]
        public async Task RatesFromAmdToUsdIsHigherThanRatesFromUsdToAmd()
        {
            // Act
            var exchangePoint = await Execute();
        
            // Assert
            var amdToUsdConversion = new Conversion() { From = Currency.Amd, To = Currency.Usd };
            var usdToAmdConversion = new Conversion() { From = Currency.Usd, To = Currency.Amd };
        
            Assert.True(exchangePoint.Rates[amdToUsdConversion].FXRate > exchangePoint.Rates[usdToAmdConversion].FXRate);
        }
        
        [Fact]
        public async Task RatesFromAmdToEurIsHigherThanRatesFromEurToAmd()
        {
            // Act
            var exchangePoint = await Execute();
        
            // Assert
            var amdToEurConversion = new Conversion() { From = Currency.Amd, To = Currency.Eur };
            var eurToAmdConversion = new Conversion() { From = Currency.Eur, To = Currency.Amd };
        
            Assert.True(exchangePoint.Rates[amdToEurConversion].FXRate > exchangePoint.Rates[eurToAmdConversion].FXRate);
        }
        
        [Fact]
        public async Task RatesFromAmdToRurIsHigherThanRatesFromRurToAmd()
        {
            // Act
            var exchangePoint = await Execute();
        
            // Assert
            var amdToRurConversion = new Conversion() { From = Currency.Amd, To = Currency.Rur };
            var rurToAmdConversion = new Conversion() { From = Currency.Rur, To = Currency.Amd };
        
            Assert.True(exchangePoint.Rates[amdToRurConversion].FXRate > exchangePoint.Rates[rurToAmdConversion].FXRate);
        }
        
        private async Task<ExchangePoint> Execute()
        {
            var exchangePoint =
                await new SasSiteParser(new CurrencyParser(new Dictionary<string, string>()), CultureInfo.InvariantCulture)
                    .ParseAsync(
                        "https://www.sas.am/food/en/");
            return exchangePoint;
        }
    }
}