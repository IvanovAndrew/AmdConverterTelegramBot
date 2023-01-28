using System.Globalization;
using AmdConverterTelegramBot;
using AmdConverterTelegramBot.SiteParser;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace SiteParsersTests;

/// <summary>
/// By strange reason, tests fail on GitHub but works fine locally. TODO fix it on Github
/// </summary>
// public class ArtsakhbankRateParserTest : ArmenianBankSiteBaseTest
// {
//     protected override string BankName => "Artsakhbank";
//     protected override string Site => "https://www.artsakhbank.am/en/home";
//     protected override HtmlParserBase CreateParser(ICurrencyParser currencyParser, CultureInfo cultureInfo)
//     {
//         return new ArtsakhbankRateParser(currencyParser, cultureInfo);
//     }
// }