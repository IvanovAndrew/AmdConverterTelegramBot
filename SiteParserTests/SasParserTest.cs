using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using HtmlAgilityPack;
using Xunit;

namespace SiteParsersTests
{
    public class SasParserTest
    {
        [Fact]
        public void Parse()
        {
            // Act
            var exchangePoint = Execute();
            
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
        public void RatesFromAmdToUsdIsHigherThanRatesFromUsdToAmd()
        {
            // Act
            var exchangePoint = Execute();
        
            // Assert
            var amdToUsdConversion = new Conversion() { From = Currency.Amd, To = Currency.Usd };
            var usdToAmdConversion = new Conversion() { From = Currency.Usd, To = Currency.Amd };
        
            Assert.True(exchangePoint.Rates[amdToUsdConversion].FXRate > exchangePoint.Rates[usdToAmdConversion].FXRate);
        }
        
        [Fact]
        public void RatesFromAmdToEurIsHigherThanRatesFromEurToAmd()
        {
            // Act
            var exchangePoint = Execute();
        
            // Assert
            var amdToEurConversion = new Conversion() { From = Currency.Amd, To = Currency.Eur };
            var eurToAmdConversion = new Conversion() { From = Currency.Eur, To = Currency.Amd };
        
            Assert.True(exchangePoint.Rates[amdToEurConversion].FXRate > exchangePoint.Rates[eurToAmdConversion].FXRate);
        }
        
        [Fact]
        public void RatesFromAmdToRurIsHigherThanRatesFromRurToAmd()
        {
            // Act
            var exchangePoint = Execute();
        
            // Assert
            var amdToRurConversion = new Conversion() { From = Currency.Amd, To = Currency.Rur };
            var rurToAmdConversion = new Conversion() { From = Currency.Rur, To = Currency.Amd };
        
            Assert.True(exchangePoint.Rates[amdToRurConversion].FXRate > exchangePoint.Rates[rurToAmdConversion].FXRate);
        }
        
        private ExchangePoint Execute()
        {
            var parser = new SasSiteParser(new CurrencyParser(), CultureInfo.InvariantCulture);

            var htmlDoc = new HtmlWeb().Load("https://www.sas.am/food/en/");

            var exchangePoint = parser.Parse(htmlDoc.ParsedText, true);
            return exchangePoint.Value;
        }
    }
}