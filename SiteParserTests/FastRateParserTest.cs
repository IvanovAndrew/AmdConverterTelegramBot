using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.SiteParser.Bank;

namespace SiteParsersTests;

public class FastRateParserTest : ArmenianBankSiteBaseTest
{
    protected override string BankName => "Fast Bank";
    protected override string Site => "https://mobileapi.fcc.am/FCBank.Mobile.Api_V2/api/publicInfo/getRates?langID=1";
    protected override FastRateParser CreateParser(CurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        return new FastRateParser(currencyParser, cultureInfo);
    }
}