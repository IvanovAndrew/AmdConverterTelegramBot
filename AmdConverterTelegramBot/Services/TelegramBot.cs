using AmdConverterTelegramBot.Controllers;
using Telegram.Bot;

namespace AmdConverterTelegramBot.Services;

public class TelegramBot
{
    private readonly IConfiguration _configuration;
    private TelegramBotClient? _botClient;
    private string _token;
    private string _url;

    public TelegramBot(IConfiguration configuration) : this(configuration["Token"], configuration["Url"])
    {
        _configuration = configuration;
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

        var hook = $"{_url}{AmdConverterController.Route}";
        await _botClient.SetWebhookAsync(hook);

        return _botClient;
    }
}