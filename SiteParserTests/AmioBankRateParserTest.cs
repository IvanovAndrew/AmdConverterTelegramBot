using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class AmioBankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Amiobank";
    protected override string Site => "https://www.amiobank.am/en";

    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new AmioBankRateParser(new CurrencyParser(),
            CultureInfo.InvariantCulture);
    }

    protected override ExchangePoint Execute(bool cash)
    {
        var parser = new AmioBankRateParser(new CurrencyParser(),
            CultureInfo.InvariantCulture);

        string url = Site;

        var xmlDoc = GetString(url);

        var exchangePoint = parser.Parse(xmlDoc, cash);
        return exchangePoint.Value;
    }
}