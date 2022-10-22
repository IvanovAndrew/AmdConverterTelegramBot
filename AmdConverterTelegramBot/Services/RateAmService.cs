using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services
{
    public class RateAmService
    {
        private readonly string _sourceUrl;
        public RateAmService(string url)
        {
            _sourceUrl = url;
        }
        
        public async Task<Result<Dictionary<Bank, decimal>>> Convert(Money money, Currency currency, bool toCurrency)
        {
            if (money.Amount <= 0) return Result<Dictionary<Bank, decimal>>.Error("Money is expected to be positive!");
            if (currency != Currency.Amd && money.Currency != Currency.Amd) return Result<Dictionary<Bank, decimal>>.Error("One of the currencies must be AMD");

            var nonDramCurrency = currency == Currency.Amd ? money.Currency : currency;
            
            var bankRatesLoadingResult = await LoadBankRates(_sourceUrl, nonDramCurrency);
            
            if (!bankRatesLoadingResult.IsSuccess) return Result<Dictionary<Bank, decimal>>.Error(bankRatesLoadingResult.ErrorMessage);

            var bankRates = bankRatesLoadingResult.Value;

            var requestedBanks = bankRates;

            var result = new Dictionary<Bank, decimal>();
            foreach (var bank in requestedBanks)
            {
                if (bank.Rates[nonDramCurrency] == Rate.Unknown)
                {
                    result[bank] = decimal.MaxValue;
                    continue;
                }
                    
                result[bank] = AmdConverter.Convert(money, currency, bank.Rates[nonDramCurrency], toCurrency);
            }

            return Result<Dictionary<Bank, decimal>>.Ok(result);
        }
        
        private async Task<Result<List<Bank>>> LoadBankRates(string url, Currency currency)
        {
            List<Bank> result = new List<Bank>();
            HtmlWeb html = new HtmlWeb();
            var webPage = await html.LoadFromWebAsync(url);
            if (webPage == null)
            {
                return Result<List<Bank>>.Error($"{url} is unavailable");
            }

            var table = webPage.DocumentNode.SelectSingleNode("//table[@id='rb']");

            int bankNameIndex = 0;
            int dateIndex = 0;
            Indices indices = new Indices();
            bool indicesInitialized = false;
            
            foreach (HtmlNode row in table.SelectNodes("tr"))
            {
                var selectNodes = row.SelectNodes("th|td");
                if (!indicesInitialized && row.GetAttributes().All(c => c.Name != "id"))
                {
                    int innerIndex = 0;
                    for (int i = 0; i < selectNodes.Count; i++)
                    {
                        var cell = selectNodes[i];

                        string innerText = "";
                        if (cell.ChildNodes.Any(c => c.Name == "select"))
                        {
                            foreach (var childNode in cell.ChildNodes.First(c => c.Name == "select").ChildNodes)
                            {
                                if (childNode.GetAttributeValue("selected", "") == "selected")
                                {
                                    innerText = childNode.InnerText?? String.Empty;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            innerText = (cell.InnerText?? String.Empty).Trim();
                        }
                        
                        if (new[] { "Bank" }.Contains(innerText))
                        {
                            bankNameIndex = innerIndex;
                        }
                        else if (new[] { "Date" }.Contains(innerText))
                        {
                            dateIndex = innerIndex;
                        }
                        else if (new[] { $"1 {currency.Name}"}.Contains(innerText))
                        {
                            indices = new Indices(innerIndex, innerIndex + 1);
                        }

                        int shift = selectNodes[i].GetAttributeValue("colspan", 1);

                        innerIndex += shift;
                    }

                    indicesInitialized = true;
                    continue;
                }

                else if (row.GetAttributes().Any(c => c.Name == "id"))
                {
                    var values = selectNodes.Select(n => n.InnerText.Trim()).ToArray();

                    var bank = new Bank
                    {
                        Name = values[bankNameIndex]
                    };
                    var rate = ParseRate(values, indices);
                    bank.AddRate(currency, rate);

                    result.Add(bank);
                }
            }

            return Result<List<Bank>>.Ok(result);
        }

        private Rate ParseRate(string[] values, Indices indices)
        {
            if (string.IsNullOrEmpty(values[indices.Buy]) || string.IsNullOrEmpty(values[indices.Sell]))
                    return Rate.Unknown;
            
            return new Rate(decimal.Parse(values[indices.Buy]), decimal.Parse(values[indices.Sell]));
        }
        
        private struct Indices
        {
            internal readonly int Buy;
            internal readonly int Sell;

            internal Indices(int buy, int sell)
            {
                Buy = buy;
                Sell = sell;
            }
        }
    }

    public static class AmdConverter
    {
        public static decimal Convert(Money money, Currency currency, Rate rate, bool toCurrency)
        {
            if (money.Currency == Currency.Amd)
            {
                return toCurrency? money.Amount / rate.Sell : money.Amount / rate.Buy;
            } 
            else if (currency == Currency.Amd)
            {
                return toCurrency ? money.Amount * rate.Buy : money.Amount * rate.Sell;
            }

            throw new ArgumentException("One of the currencies should be AMD!");
        }
    }
}

