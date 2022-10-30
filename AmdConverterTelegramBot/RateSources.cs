namespace AmdConverterTelegramBot;

public class RateSources
{
    public IReadOnlyDictionary<string, BankInfo> Banks { get; }
    public string RateamCashUrl { get; }
    public string RateamNonCashUrl { get; }
    public string SasUrl { get; }

    public RateSources(IConfiguration configuration)
    {
        Banks = configuration.GetSection("Banks").Get<BankInfo[]>().ToDictionary(b => b.Name.ToLowerInvariant());
        RateamCashUrl = configuration["RateSources:RateamCashUrl"];
        RateamNonCashUrl = configuration["RateSources:RateamNonCashUrl"];
        SasUrl = configuration["RateSources:SasUrl"];
    }
}

public class BankInfo
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Url { get; set; }
}