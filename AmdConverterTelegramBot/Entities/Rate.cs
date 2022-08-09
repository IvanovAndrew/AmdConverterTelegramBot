namespace AmdConverterTelegramBot.Entities;

public class Rate
{
    public readonly decimal Buy;
    public readonly decimal Sell;
    public readonly DateTime Date;

    public static readonly Rate Unknown = new Rate(decimal.MaxValue, decimal.MaxValue);

    public Rate(/*DateTime date,*/ decimal buy, decimal sell)
    {
        if (buy <= 0) throw new ArgumentOutOfRangeException(nameof(buy));
        if (sell <= 0) throw new ArgumentOutOfRangeException(nameof(sell));
        
        Buy = buy;
        Sell = sell;
        Date = DateTime.Now;
    }
}