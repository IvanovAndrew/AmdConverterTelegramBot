using System.Globalization;

namespace AmdConverterTelegramBot.Shared.Entities;

public class Rate
{
    public readonly decimal FXRate;
    public static readonly Rate Unknown = new (decimal.MaxValue);

    public Rate(decimal value)
    {
        if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
        FXRate = value;
    }

    public static bool TryParse(string s, out Rate rate)
    {
        return TryParse(s, new CultureInfo("en-us"), out rate);
    }
    
    public static bool TryParse(string s, CultureInfo cultureInfo, out Rate rate)
    {
        rate = Unknown;
        if (decimal.TryParse(s, NumberStyles.Any, cultureInfo, out decimal value) && value > 0)
        {
            rate = new Rate(value);
            return true;
        }

        return false;
    }
}