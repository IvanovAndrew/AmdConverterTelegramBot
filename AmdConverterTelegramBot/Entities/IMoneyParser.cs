namespace AmdConverterTelegramBot.Entities;

public interface IMoneyParser
{
    bool TryParse(string text, out Money money);
}