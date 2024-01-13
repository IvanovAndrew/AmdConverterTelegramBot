using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.SiteParser;

namespace AmdConverterTelegramBot.Services;

public class RateLoader
{
    private readonly IBankParserFactory _parserFactory;
    private readonly RateAmParser _rateAmParser;
    private readonly RateSources _rateSources;
    private readonly ILogger _logger;

    public RateLoader(IBankParserFactory parserFactory, RateAmParser rateAmParser, RateSources rateSources, ILogger logger)
    {
        _parserFactory = parserFactory ?? throw new ArgumentNullException(nameof(parserFactory));
        _rateAmParser = rateAmParser ?? throw new ArgumentNullException(nameof(rateAmParser));
        _rateSources = rateSources ?? throw new ArgumentNullException(nameof(rateSources));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ExchangePoint>>> LoadRates(bool cash)
    {
        Task<Result<List<ExchangePoint>>> rateAmTask, ratesFromBankSites;
        using (HttpClient httpClient = new HttpClient(new HttpClientHandler{AllowAutoRedirect = true, MaxAutomaticRedirections = 2}))
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");
            rateAmTask = RatesFromRateAm(httpClient, cash);
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

    private async Task<Result<List<ExchangePoint>>> RatesFromRateAm(HttpClient httpClient, bool cash)
    {
        var rateAmHtml = await GetStringAsync(httpClient, cash? _rateSources.RateamCashUrl : _rateSources.RateamNonCashUrl);
        
        return rateAmHtml.Bind(html => _rateAmParser.Parse(html));
    }

    private async Task<Result<List<ExchangePoint>>> RatesFromSites(HttpClient httpClient, bool cash)
    {
        // https://dotnettutorials.net/lesson/how-to-execute-multiple-tasks-in-csharp/
        var tasks = new List<Task<Result<ExchangePoint>>>();
        foreach (var (bank, info) in  _rateSources.Banks)
        {
            var rateTask = RateFromSite(httpClient, (cash ? info.CashUrl : info.NonCashUrl) ?? info.Url, bank, cash);
            tasks.Add(rateTask);
        }
        
        var sasTask = RateFromSite(httpClient, _rateSources.SasUrl, "SAS", cash);
        tasks.Add(sasTask);

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

    private async Task<Result<string>> GetStringAsync(HttpClient httpClient, string url)
    {
        try
        {
            var result = await httpClient.GetStringAsync(url);
            return Result<string>.Ok(result);
        }
        catch(Exception e)
        {
            _logger.LogError($"Couldn't parse {url}", e);
            return Result<string>.Error($"Couldn't parse {url}");
        }
    }

    private async Task<Result<ExchangePoint>> RateFromSite(HttpClient httpClient, string url, string bank, bool cash)
    {
        var source = await GetStringAsync(httpClient, url);

        return source.Bind(html => LoadRate(bank, html, cash));
    }
    
    private Result<ExchangePoint> LoadRate(string bank, string source, bool cash)
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
}