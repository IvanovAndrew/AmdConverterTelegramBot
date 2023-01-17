using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

// TODO implement parser and uncomment test
// public class AmeriabankRateParserTest : ArmenianBankSiteBaseTest
// {
//     protected override string BankName => "Ameriabank";
//     protected override string Site => "https://ameriabank.am/en";
//
//     protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
//     {
//         return new AmeriabankRatesParser(currencyParser, cultureInfo);
//     }
// }