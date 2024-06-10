namespace AmdConverterTelegramBot.Shared.SiteParser;

public class RateAmParser
{
    private readonly CurrencyParser _currencyParser;
    public RateAmParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        _currencyParser = currencyParser?? throw new ArgumentNullException(nameof(currencyParser));
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
            codeToBank[organization.Name] = organization.Value["name"].ToString().Trim();
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
                        var sellRate = jsonRates["sell"].ToString();
                        if (!string.IsNullOrEmpty(sellRate))
                        {
                            bank.AddRate(new Conversion {From = Currency.Amd, To = currency}, new Rate(decimal.Parse(sellRate)));
                        }

                        var buyRate = jsonRates["buy"].ToString();
                        if (!string.IsNullOrEmpty(buyRate))
                        {
                            bank.AddRate(new Conversion {From = currency, To = Currency.Amd}, new Rate(decimal.Parse(buyRate)));
                        }
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