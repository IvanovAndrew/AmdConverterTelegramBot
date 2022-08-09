namespace AmdConverterTelegramBot;

public class RateAmOptions
{
    public IReadOnlyDictionary<string, BankInfo> Banks { get; }

    public RateAmOptions(IConfiguration configuration)
    {
        Banks = configuration.GetSection("Banks").Get<BankInfo[]>().ToDictionary(b => b.Name.ToLowerInvariant());
    }
}

public class BankInfo
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Url { get; set; }
}