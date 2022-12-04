namespace AmdConverterTelegramBot.Entities;

public class Rate
{
    public readonly decimal FXRate;
    public static readonly Rate Unknown = new Rate(decimal.MaxValue);

    public Rate(decimal value)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
        FXRate = value;
    }
}