using System.Collections.Generic;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using Xunit;

namespace AmdConverterTelegramBotTests;

public class AmdConverterTests
{
    public static IEnumerable<object[]> ConvertMoneyToCurrencyData => 
        new List<object[]>
        {
            // I have 100$. If I convert them to AMD, I'll get 40 000 AMD
            new object[] { new Money(){Currency = Currency.Usd, Amount = 100}, Currency.Amd, new Rate(400, 500), 40_000 },
            // I have 500AMD. If convert them to $, I'll get 1$.
            new object[] { new Money(){Currency = Currency.Amd, Amount = 500}, Currency.Usd, new Rate(400, 500), 1 },
        };
    
    public static IEnumerable<object[]> ConvertCurrencyToMoneyData => 
        new List<object[]>
        {
            // I want to have 100$. 50 000 AMD is enough
            new object[] { new Money(){Currency = Currency.Usd, Amount = 100}, Currency.Amd, new Rate(400, 500), 50_000 },
            // I want to have 500 AMD. 1.25$ is enough
            new object[] { new Money(){Currency = Currency.Amd, Amount = 500}, Currency.Usd, new Rate(400, 500), 1.25 },
        };
    
    [Theory]
    [MemberData(nameof(ConvertMoneyToCurrencyData))]
    public void ConvertMoneyToCurrency(Money money, Currency currency, Rate rate, decimal expected)
    {
        // Act
        var result = AmdConverter.Convert(money, currency, rate, true);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(ConvertCurrencyToMoneyData))]
    public void ConvertCurrencyToMoney(Money money, Currency currency, Rate rate, decimal expected)
    {
        // Act
        var result = AmdConverter.Convert(money, currency, rate, false);
        
        // Assert
        Assert.Equal(expected, result);
    }
}