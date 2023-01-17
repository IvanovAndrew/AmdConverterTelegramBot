using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

public class MellatbankRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Mellat bank";
    protected override string Site => "https://api.mellatbank.am/api/v1/rate/list?";
    protected override RateParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new MellatbankRateParser(currencyParser, cultureInfo);
    }
}