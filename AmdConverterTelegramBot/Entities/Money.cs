namespace AmdConverterTelegramBot.Entities
{
    public record Money
    {
        public Currency Currency { get; init; }
        public decimal Amount { get; init; }
    
        public override string ToString() => $"{Amount}{Currency.Symbol}";
    }
}

