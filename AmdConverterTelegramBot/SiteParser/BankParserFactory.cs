using System.Globalization;
using AmdConverterTelegramBot.SiteParser.Bank;

namespace AmdConverterTelegramBot.SiteParser;

public interface IBankParserFactory
{
    bool TryGetParser(string bank, out RateParserBase parser);
}

public class BankParserFactory : IBankParserFactory
{
    private Dictionary<string, RateParserBase> _mapping = new();

    private Dictionary<string, RateParserBase> Mapping
    {
        get
        {
            if (_mapping.Count == 0)
            {
                _mapping = CreateMapping();
            }

            return _mapping;
        }
    }

    private readonly ICurrencyParser _currencyParser;
    private readonly CultureInfo _cultureInfo;
    
    public BankParserFactory(ICurrencyParser currencyParser, CultureInfo cultureInfo)
    {
        _currencyParser = currencyParser ?? throw new ArgumentNullException(nameof(currencyParser));
        _cultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
    }
    
    public bool TryGetParser(string bank, out RateParserBase parser)
    {
        return Mapping.TryGetValue(bank, out parser);
    }

    private Dictionary<string, RateParserBase> CreateMapping()
    {
        return 
            new(StringComparer.InvariantCultureIgnoreCase)
            {
                ["Acba bank"] = new AcbaSiteParser(_currencyParser, _cultureInfo),
                ["Ameriabank"] = new AmeriabankRatesParser(_currencyParser, _cultureInfo),
                ["Araratbank"] = new AraratbankRateParser(_currencyParser, _cultureInfo),
                ["Ardshinbank"] = new ArdshinbankSiteParser(_currencyParser, _cultureInfo),
                ["Amiobank"] = new AmioBankRateParser(_currencyParser, _cultureInfo),
                ["ARMECONOMBANK"] = new ArmeconombankRateParser(_currencyParser, _cultureInfo),
                ["ArmSwissBank"] = new ArmswissbankSiteParser(_currencyParser, _cultureInfo),
                ["Artsakhbank"] = new ArtsakhbankRateParser(_currencyParser, _cultureInfo),
                ["Byblos Bank Armenia"] = new ByblosRateParser(_currencyParser, _cultureInfo),
                ["Converse Bank"] = new ConverseRateParser(_currencyParser, _cultureInfo),
                ["Evocabank"] = new EvocabankSiteParser(_currencyParser, _cultureInfo),
                ["Fast Bank"] = new FastRateParser(_currencyParser, _cultureInfo),
                ["HSBC Bank Armenia"] = new HSBCRateParser(_currencyParser, _cultureInfo),
                ["IDBank"] = new IdbankRateParser(_currencyParser, _cultureInfo),
                ["Inecobank"] = new InecobankRateParser(_currencyParser, _cultureInfo),
                ["Mellat Bank"] = new MellatbankRateParser(_currencyParser, _cultureInfo),
                ["Unibank"] = new UnibankRateParser(_currencyParser, _cultureInfo),
                ["VTB Bank (Armenia)"] = new VTBRateParser(_currencyParser, _cultureInfo),
                
                ["SAS"] = new SasSiteParser(_currencyParser, _cultureInfo),
                ["MIR"] = new MirSiteParser()
            };
    }
}