namespace AmdConverterTelegramBot.Entities;

public class Bank
{
    public string Name { get; init; }
    public Dictionary<Currency, Rate> Rates { get; } = new();

    public void AddRate(Currency currency, Rate rate) => Rates[currency] = rate;
}