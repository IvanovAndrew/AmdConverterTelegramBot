using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.Services;

public class Parser
{
    private readonly RateAmParser _rateAmParser;
    private readonly SasSiteParser _sasSiteParser;
    private readonly MirSiteParser _mirSiteParser;
    private readonly RateSources _rateSources;

    public Parser(RateAmParser rateAmParser, SasSiteParser sasSiteParser, MirSiteParser mirSiteParser, RateSources rateSources)
    {
        _rateAmParser = rateAmParser ?? throw new ArgumentNullException(nameof(rateAmParser));
        _sasSiteParser = sasSiteParser ?? throw new ArgumentNullException(nameof(sasSiteParser));
        _mirSiteParser = mirSiteParser ?? throw new ArgumentNullException(nameof(mirSiteParser));
        _rateSources = rateSources ?? throw new ArgumentNullException(nameof(rateSources));
    }

    public async Task<Result<List<ExchangePoint>>> LoadCashRates()
    {
        var bankRates = await _rateAmParser.ParseAsync(_rateSources.RateamCashUrl);

        var sasRates = await _sasSiteParser.ParseAsync(_rateSources.SasUrl);

        if (bankRates.IsSuccess)
        {
            bankRates.Value.Add(sasRates);
        }

        var mirRates = await _mirSiteParser.ParseAsync(_rateSources.MirUrl);
        bankRates.Value.Add(mirRates);

        return bankRates;
    }

    public async Task<Result<List<ExchangePoint>>> LoadNonCashRates()
    {
        var rates = await _rateAmParser.ParseAsync(_rateSources.RateamNonCashUrl);

        if (rates.IsSuccess)
        {
            var mirRates = await _mirSiteParser.ParseAsync(_rateSources.MirUrl);
            rates.Value.Add(mirRates);
        }

        return rates;
    }
}