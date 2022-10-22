using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace AmdConverterTelegramBot;

public class RateAmOptions
{
    public IReadOnlyDictionary<string, BankInfo> Banks { get; }
    public string CashUrl { get; }
    public string NonCashUrl { get; }

    public RateAmOptions(IConfiguration configuration)
    {
        Banks = configuration.GetSection("Banks").Get<BankInfo[]>().ToDictionary(b => b.Name.ToLowerInvariant());
        CashUrl = configuration["RateAmUrl:Cash"];
        NonCashUrl = configuration["RateAmUrl:NonCash"];
    }
}

public class BankInfo
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Url { get; set; }
}