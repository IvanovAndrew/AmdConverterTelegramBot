using AmdConverterTelegramBot.Shared.Entities;

namespace AmdConverterTelegramBot.Entities;

public class ConversionInfo
{
    public ExchangePoint ExchangePoint { get; init; }
    public Conversion Conversion { get; init; }
    public Rate Rate { get; init; }
    public Money FromMoney { get; init; }
    public Money ToMoney { get; init; }
}