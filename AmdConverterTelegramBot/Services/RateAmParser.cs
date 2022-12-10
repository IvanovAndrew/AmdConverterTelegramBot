using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;

namespace AmdConverterTelegramBot.Services
{
    public class RateAmParser
    {
        private readonly IMoneyParser _moneyParser;
        private readonly CultureInfo _cultureInfo;
        public RateAmParser(IMoneyParser moneyParser, CultureInfo cultureInfo)
        {
            _moneyParser = moneyParser?? throw new ArgumentNullException(nameof(moneyParser));
            _cultureInfo = cultureInfo?? throw new ArgumentNullException(nameof(cultureInfo));
        }

        public Result<List<ExchangePoint>> Parse(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            return Parse(htmlDocument);
        }

        public Result<List<ExchangePoint>> Parse(HtmlDocument webPage)
        {
            List<ExchangePoint> result = new();

            var table = webPage.DocumentNode.SelectSingleNode("//table[@id='rb']");

            int bankNameIndex = 0;
            int dateIndex = 0;
            Dictionary<Currency, Indices> currencyToIndices = new Dictionary<Currency, Indices>();
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
                        else if (_moneyParser.TryParse(innerText, out var money))
                        {
                            currencyToIndices[money.Currency] = new Indices(innerIndex, innerIndex + 1);
                        }

                        int shift = selectNodes[i].GetAttributeValue("colspan", 1);

                        innerIndex += shift;
                    }

                    indicesInitialized = true;
                }

                else if (row.GetAttributes().Any(c => c.Name == "id"))
                {
                    var values = selectNodes.Select(n => n.InnerText.Trim()).ToArray();

                    var bank = new ExchangePoint()
                    {
                        Name = values[bankNameIndex],
                        BaseCurrency = Currency.Amd,
                    };

                    foreach (var (currency, indices) in currencyToIndices)
                    {
                        var rate = ParseRate(values, indices.Sell);
                        bank.AddRate(new Conversion {From = Currency.Amd, To = currency}, rate);
                        
                        rate = ParseRate(values, indices.Buy);
                        bank.AddRate(new Conversion {From = currency, To = Currency.Amd}, rate);
                    }

                    result.Add(bank);
                }
            }

            return Result<List<ExchangePoint>>.Ok(result);
        }

        private Rate ParseRate(string[] values, int index)
        {
            if (string.IsNullOrEmpty(values[index]))
                return Rate.Unknown;
        
            return new Rate(decimal.Parse(values[index], _cultureInfo));
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
}

