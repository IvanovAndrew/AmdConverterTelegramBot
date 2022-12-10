using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.Services;

public class RateLoader
{
    private readonly RateAmParser _rateAmParser;
    private readonly SasSiteParser _sasSiteParser;
    private readonly MirSiteParser _mirSiteParser;
    private readonly RateSources _rateSources;
    private readonly ILogger _logger;

    public RateLoader(RateAmParser rateAmParser, SasSiteParser sasSiteParser, MirSiteParser mirSiteParser, RateSources rateSources)
    {
        _rateAmParser = rateAmParser ?? throw new ArgumentNullException(nameof(rateAmParser));
        _sasSiteParser = sasSiteParser ?? throw new ArgumentNullException(nameof(sasSiteParser));
        _mirSiteParser = mirSiteParser ?? throw new ArgumentNullException(nameof(mirSiteParser));
        _rateSources = rateSources ?? throw new ArgumentNullException(nameof(rateSources));
        // _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ExchangePoint>>> LoadCashRates()
    {
        string rateAmHtml, sasHtml = "", mirHtml = "";
        using (HttpClient httpClient = new HttpClient())
        {
            rateAmHtml = await GetStringAsync(httpClient, _rateSources.RateamCashUrl);
            sasHtml = await GetStringAsync(httpClient, _rateSources.SasUrl);
            mirHtml = await GetStringAsync(httpClient, _rateSources.MirUrl);
        }
        
        var bankRates = _rateAmParser.Parse(rateAmHtml);

        if (bankRates.IsSuccess)
        {
            if (sasHtml != string.Empty)
            {
                var sasRates = _sasSiteParser.Parse(sasHtml);
                bankRates.Value.Add(sasRates);
            }

            if (mirHtml != string.Empty)
            {
                var mirRates = _mirSiteParser.Parse(mirHtml);
                bankRates.Value.Add(mirRates);
            }
        }

        return bankRates;
    }

    private async Task<string> GetStringAsync(HttpClient httpClient, string url)
    {
        try
        {
            return await httpClient.GetStringAsync(url);
        }
        catch(Exception e)
        {
            return await Task.FromResult("");
            // _logger.LogError($"Couldn't parse {url}", e);
        }
    }

    public async Task<Result<List<ExchangePoint>>> LoadNonCashRates()
    {
        string rateAmHtml, mirHtml;
        using (HttpClient httpClient = new HttpClient())
        {
            rateAmHtml = await GetStringAsync(httpClient, _rateSources.RateamNonCashUrl);
            mirHtml = await GetStringAsync(httpClient, _rateSources.MirUrl);
        }
        
        var rates = _rateAmParser.Parse(rateAmHtml);

        if (rates.IsSuccess && mirHtml != String.Empty)
        {
            var mirRates = _mirSiteParser.Parse(mirHtml);
            rates.Value.Add(mirRates);
        }

        return rates;
    }
}