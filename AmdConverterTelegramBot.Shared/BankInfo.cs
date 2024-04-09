namespace AmdConverterTelegramBot.Shared;

public abstract class BankInfo
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string Url { get; set; }
    public string? CashUrl { get; set; }
    public string? NonCashUrl { get; set; }
}