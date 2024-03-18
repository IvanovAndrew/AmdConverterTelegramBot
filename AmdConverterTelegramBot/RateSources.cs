namespace AmdConverterTelegramBot;

public class RateSources
{
    public IReadOnlyDictionary<string, BankInfo> Banks { get; }
    public string RateamUrl { get; }
    public string SasUrl { get; }
    public string MirUrl { get; }

    public RateSources(IConfiguration configuration)
    {
        Banks = configuration.GetSection("Banks").Get<BankInfo[]>().ToDictionary(b => b.Name.ToLowerInvariant());
        RateamUrl = configuration["RateSources:RateamUrl"];
        SasUrl = configuration["RateSources:SasUrl"];
        MirUrl = configuration["RateSources:MirUrl"];
    }
}

public class BankInfo
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Url { get; set; }
    public string? CashUrl { get; set; }
    public string? NonCashUrl { get; set; }
}