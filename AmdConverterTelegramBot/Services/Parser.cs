using AmdConverterTelegramBot.Entities;

namespace AmdConverterTelegramBot.Services;

public class Parser
{
    private readonly RateAmParser _rateAmParser;
    private readonly SasSiteParser _sasSiteParser;
    private readonly RateSources _rateSources;

    public Parser(RateAmParser rateAmParser, SasSiteParser sasSiteParser, RateSources rateSources)
    {
        _rateAmParser = rateAmParser ?? throw new ArgumentNullException(nameof(rateAmParser));
        _sasSiteParser = sasSiteParser ?? throw new ArgumentNullException(nameof(sasSiteParser));
        _rateSources = rateSources ?? throw new ArgumentNullException(nameof(rateSources));
    }

    public async Task<Result<List<Bank>>> LoadCashRates()
    {
        var bankRates = await _rateAmParser.ParseAsync(_rateSources.RateamCashUrl);

        var sasRates = await _sasSiteParser.ParseAsync(_rateSources.SasUrl);

        if (bankRates.IsSuccess)
        {
            bankRates.Value.Add(sasRates);
        }

        return bankRates;
    }

    public async Task<Result<List<Bank>>> LoadNonCashRates()
    {
        return await _rateAmParser.ParseAsync(_rateSources.RateamNonCashUrl);
    }
}