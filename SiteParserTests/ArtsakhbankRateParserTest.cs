using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

/// <summary>
/// By strange reason, tests fail on GitHub but works fine locally. TODO fix it on Github
/// </summary>
public class ArtsakhbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Artsakhbank";
    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new ArtsakhbankRateParser(currencyParser, cultureInfo);
    }
}