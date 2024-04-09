using Telegram.Bot;

namespace AmdConverterTelegramBot.Shared.Services;

public class TelegramBot
{
    private TelegramBotClient? _botClient;
    private readonly string _token;
    private readonly string _url;

    public TelegramBot(string token)
    {
        _token = token;
    }

    public TelegramBot(string token, string url)
    {
        _token = token;
        _url = url.Last() == '/' ? url : url + "/";
    }

    public async Task<TelegramBotClient> GetBot()
    {
        if (_botClient != null)
        {
            return _botClient;
        }
            
        _botClient = new TelegramBotClient(_token);

        // var hook = $"{_url}{AmdConverterController.Route}";
        // await _botClient.SetWebhookAsync(hook);

        return _botClient;
    }
}