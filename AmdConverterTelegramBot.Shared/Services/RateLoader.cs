using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.SiteParser;
using Microsoft.Extensions.Logging;

namespace AmdConverterTelegramBot.Shared.Services;

public class RateLoader
{
    private readonly IBankParserFactory _parserFactory;
    private readonly RateAmLoader _rateAmLoader;
    private readonly ILogger _logger;

    public RateLoader(IBankParserFactory parserFactory, RateAmLoader rateAmParser, ILogger logger)
    {
        _parserFactory = parserFactory ?? throw new ArgumentNullException(nameof(parserFactory));
        _rateAmLoader = rateAmParser ?? throw new ArgumentNullException(nameof(rateAmParser));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ExchangePoint>>> LoadRates(bool cash)
    {
        Task<Result<List<ExchangePoint>>> rateAmTask, ratesFromBankSites;
        
        using (HttpClient httpClient = new HttpClient(new HttpClientHandler{AllowAutoRedirect = true}))
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");
            rateAmTask = _rateAmLoader.GetRatesAsync(httpClient, cash);
            ratesFromBankSites = RatesFromSites(httpClient, cash);
            
            await Task.WhenAll(rateAmTask, ratesFromBankSites);
        }

        var rateAmRates = rateAmTask.Result.ValueOrDefault(new List<ExchangePoint>());
        var bankRates = ratesFromBankSites.Result.ValueOrDefault(new List<ExchangePoint>());

        var result = new Dictionary<string, ExchangePoint>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var exchangePoint in rateAmRates.Union(bankRates))
        {
            if (result.TryGetValue(exchangePoint.Name, out var ep))
            {
                foreach (var (conversion, rate) in exchangePoint.Rates)
                {
                    if (!ep.Rates.ContainsKey(conversion))
                    {
                        ep.AddRate(conversion, rate);
                    }
                }
            }
            else
            {
                result[exchangePoint.Name] = exchangePoint;
            }
        }
        
        return result.Any()? Result<List<ExchangePoint>>.Ok(result.Values.ToList()) : Result<List<ExchangePoint>>.Error("Couldn't get any data");
    }

    private async Task<Result<List<ExchangePoint>>> RatesFromSites(HttpClient httpClient, bool cash)
    {
        var allBanks = GetAllBanks();
        // https://dotnettutorials.net/lesson/how-to-execute-multiple-tasks-in-csharp/
        var tasks = new List<Task<Result<ExchangePoint>>>();
        foreach (var bank in allBanks)
        {
            var rateTask = bank.GetStringAsync(httpClient, cash);
            tasks.Add(rateTask);
        }
        
        await Task.WhenAll(tasks);

        var exchangePoints = new List<ExchangePoint>();
        var errors = new List<string>();

        foreach (var task in tasks)
        {
            var result = task.Result;
            result.IterValue(value => exchangePoints.Add(value));
            result.IterError(error => errors.Add(error));
        }

        return exchangePoints.Any()? Result<List<ExchangePoint>>.Ok(exchangePoints) : Result<List<ExchangePoint>>.Error(string.Join(Environment.NewLine, errors));
    }

    private Result<ExchangePoint> Parse(string bank, string source, bool cash)
    {
        if (_parserFactory.TryGetParser(bank, out var parser))
        {
            try
            {
                return parser.Parse(source, cash);
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occured during parsing {bank}: {e}");
                return Result<ExchangePoint>.Error($"An error occured during parsing {bank}: {e}");
            }
        }
        
        _logger.LogDebug($"Parser for {bank} is not found");
        return Result<ExchangePoint>.Error($"Parser for {bank} is not found");
    }

    private List<BankRateLoader> GetAllBanks()
    {
        var type = typeof(RateParserBase);
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
            .Select(type => _parserFactory.CreateParserFromType(type))
            .Select(parser => new BankRateLoader(parser))
            .ToList();
    }
}