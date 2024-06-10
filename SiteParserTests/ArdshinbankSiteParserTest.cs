using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class ArdshinbankSiteParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Ardshinbank";
    protected override string Site => "https://website-api.ardshinbank.am/currency";

    protected override RateParserBase CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo) =>
        new ArdshinbankSiteParser(currencyParser, cultureInfo);
}