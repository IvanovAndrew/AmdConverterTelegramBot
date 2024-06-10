using Telegram.Bot;

namespace AmdConverterTelegramBot.Shared.Services;

public class TelegramBot
{
    private TelegramBotClient? _botClient;
    private readonly string _token;

    public TelegramBot(string token)
    {
        _token = token;
    }

    public TelegramBotClient GetBot()
    {
        if (_botClient != null)
        {
            return _botClient;
        }
            
        _botClient = new TelegramBotClient(_token);

        return _botClient;
    }
}