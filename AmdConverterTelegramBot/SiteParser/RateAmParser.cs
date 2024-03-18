using System.Globalization;
using AmdConverterTelegramBot.Entities;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AmdConverterTelegramBot.Services
{
    public class RateAmParser
    {
        private readonly ICurrencyParser _currencyParser;
        private readonly CultureInfo _cultureInfo;
        public RateAmParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
        {
            _currencyParser = currencyParser?? throw new ArgumentNullException(nameof(currencyParser));
            _cultureInfo = cultureInfo?? throw new ArgumentNullException(nameof(cultureInfo));
        }

        public Result<List<ExchangePoint>> Parse(string html, bool cash)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            return Parse(htmlDocument, cash);
        }

        public Result<List<ExchangePoint>> Parse(HtmlDocument webPage, bool cash)
        {
            List<ExchangePoint> result = new();

            var fullHtml = webPage.DocumentNode.OuterHtml;

            string json = ExtractJson(fullHtml);
            
            dynamic parsedJson = JsonConvert.DeserializeObject(json);


            var codeToBank = new Dictionary<string, string>();

            foreach (var organization in parsedJson["organizations"])
            {
                codeToBank[organization.Name] = organization.Value["name"].ToString();
            }

            var name = cash ? "CASH" : "CARDTRANSACTION";

            foreach (var bankRates in parsedJson["organizationRates"])
            {
                var bank = new ExchangePoint()
                {
                    Name = codeToBank[bankRates.Name],
                    BaseCurrency = Currency.Amd,
                };

                foreach (var rate in bankRates.Value["rates"])
                {
                    if (_currencyParser.TryParse(rate.Name.ToString(), out Currency currency))
                    {
                        var jsonRates = rate.Value[name];
                        if (jsonRates != null)
                        {
                            bank.AddRate(new Conversion {From = Currency.Amd, To = currency}, new Rate(decimal.Parse(jsonRates["sell"].ToString())));
                            bank.AddRate(new Conversion {From = currency, To = Currency.Amd}, new Rate(decimal.Parse(jsonRates["buy"].ToString())));
                        }
                    }
                }
                
                result.Add(bank);
            }

            return Result<List<ExchangePoint>>.Ok(result);
        }

        private string ExtractJson(string fullHtml)
        {
            var index = fullHtml.IndexOf("{\\\"lang\\\":\\\"en\\\",\\\"organizationRates\\\"");

            int length = 0;

            int bracketsCounter = 0;
            bool isStart = true;

            var fullLength = fullHtml.Length;
            for (int i = index; i < fullLength; i++)
            {
                if (!isStart && bracketsCounter == 0)
                {
                    break;
                }

                length++;

                if (fullHtml[i] == '{') bracketsCounter++;
                else if (fullHtml[i] == '}') bracketsCounter--; 
                
                isStart = false;
            }

            return fullHtml.Substring(index, length).Replace("\\", String.Empty);
        }
    }
}

