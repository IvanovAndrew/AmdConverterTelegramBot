namespace AmdConverterTelegramBot.Entities;

public record Conversion
{
    public Currency From { get; init; }
    public Currency To { get; init; }

    public override string ToString()
    {
        return $"{From.Symbol} -> {To.Symbol}";
    }
}