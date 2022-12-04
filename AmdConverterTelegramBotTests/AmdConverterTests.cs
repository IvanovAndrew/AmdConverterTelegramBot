using System;
using System.Collections.Generic;
using AmdConverterTelegramBot;
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
            new object[]
            {
                new Money{Currency = Currency.Usd, Amount = 100}, 
                new Conversion{From = Currency.Usd, To = Currency.Amd}, 
                new Money {Amount = 40_000, Currency = Currency.Amd}
            },
            // I have 500AMD. If convert them to $, I'll get 1$.
            new object[]
            {
                new Money{Currency = Currency.Amd, Amount = 500}, 
                new Conversion {From = Currency.Amd, To = Currency.Usd}, 
                new Money{Amount = 1, Currency = Currency.Usd}
            },
        };
    
    public static IEnumerable<object[]> ConvertCurrencyToMoneyData => 
        new List<object[]>
        {
            // I want to have 100$. 50 000 AMD is enough
            new object[]
            {
                new Money(){Currency = Currency.Usd, Amount = 100}, 
                new Conversion { From = Currency.Amd, To = Currency.Usd}, 
                new Money(){Amount = 50_000, Currency = Currency.Amd}
            },
            // I want to have 500 AMD. 1.25$ is enough
            new object[]
            {
                new Money(){Currency = Currency.Amd, Amount = 500}, 
                new Conversion {From = Currency.Usd, To = Currency.Amd}, 
                new Money() {Currency = Currency.Usd, Amount = 1.25m}
            },
        };

    [Theory]
    [MemberData(nameof(ConvertMoneyToCurrencyData))]
    public void ConvertMoneyToCurrency(Money sourceMoney, Conversion conversion, Money expectedMoney)
    {
        var bank = new ExchangePoint() { Name = "Unit Bank", BaseCurrency = Currency.Amd };
        bank.AddRate(new Conversion{From = Currency.Amd, To = Currency.Usd}, new Rate(500));
        bank.AddRate(new Conversion{From = Currency.Usd, To = Currency.Amd}, new Rate(400));
        
        // Act
        var result = bank.Convert(conversion, sourceMoney);
        
        // Assert
        Assert.Equal(expectedMoney, result?.ToMoney);
    }
    
    [Theory]
    [MemberData(nameof(ConvertCurrencyToMoneyData))]
    public void ConvertCurrencyToMoney(Money destinationMoney, Conversion conversion, Money expectedSourceMoney)
    {
        // Arrange
        var bank = new ExchangePoint() { Name = "Unit Bank", BaseCurrency = Currency.Amd };
        bank.AddRate(new Conversion{From = Currency.Amd, To = Currency.Usd}, new Rate(500));
        bank.AddRate(new Conversion{From = Currency.Usd, To = Currency.Amd}, new Rate(400));
        
        // Act
        var result = bank.Convert(conversion, destinationMoney);
        
        // Assert
        Assert.Equal(expectedSourceMoney, result?.FromMoney);
    }
}