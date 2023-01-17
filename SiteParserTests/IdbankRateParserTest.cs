using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class IdbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "IDbank";
    protected override string Site => "https://idbank.am/en/rates/";
    protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new IdbankRateParser(currencyParser, cultureInfo);
    }

    protected override ExchangePoint Execute(bool cash)
    {
        string htmlText;
        using (var client = new HttpClient())
        {
            var webRequest = new HttpRequestMessage(HttpMethod.Post, Site)
            {
                Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("RATE_TYPE", cash? "CASH" : "NO_CASH")
            })};
            
            var response = client.Send(webRequest);
            using var reader = new StreamReader(response.Content.ReadAsStream());
            
            htmlText = reader.ReadToEnd();
        }

        return CreateParser(new CurrencyParser(new Dictionary<string, string>() { ["RUB"] = "RUR" }),
            CultureInfo.InvariantCulture).Parse(htmlText, cash).Value;
    }
}