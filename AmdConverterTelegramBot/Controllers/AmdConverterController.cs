using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Sprache;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AmdConverterTelegramBot.Controllers;

[ApiController]
[Route(Route)]
[ApiExplorerSettings(GroupName = "Telegram")]
public class AmdConverterController : ControllerBase
{
    internal const string Route = "api/message/update";
    private const string MessageRoute = "message/update";
    private const string WebhookRoute = "webhook";
    private readonly ILogger _logger;
    private readonly TelegramBot _bot;
    private readonly RateLoader _rateLoader;
    
    private readonly Replies _replies;
    
    public AmdConverterController(ILogger<AmdConverterController> logger, TelegramBot bot, RateLoader rateLoader, Replies replies)
    {
        _logger = logger;
        _bot = bot;
        _rateLoader = rateLoader;
        _replies = replies;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        var botClient = await _bot.GetBot();

        long chatId = 0;
        int messageId = 0;
        string text = string.Empty;
        string languageCode = "en";
        if (update.Type == UpdateType.Message)
        {
            chatId = update.Message.Chat.Id;
            text =
                update.Message.Text.Replace(",", ".").ToLowerInvariant().Trim();
            messageId = update.Message.MessageId;
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            text = update.CallbackQuery.Data;
            chatId = update.CallbackQuery.Message.Chat.Id;
            messageId = update.CallbackQuery.Message.MessageId;
            languageCode = update.CallbackQuery.From.LanguageCode ?? languageCode;
        }

        text = TelegramEscaper.Decode(text);

        _logger.LogInformation($"{text} is received");
        
        if (text == "/start")
        {
            await botClient.SendTextMessageAsync(chatId, "Hello! Enter the price in AMD, USD, EUR, GEL, or RUB");
            return Ok();
        }

        try
        {
            var request = Grammar.Request.Parse(text);
            var engine = new Engine(_rateLoader, botClient, GetCultureInfo(languageCode));

            await engine.HandleRequest(request, chatId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
        
        return Ok();
    }
    
    [HttpPost]
    [Route(WebhookRoute)]
    public async Task<IActionResult> SetWebHook(string url)
    {
        var bot = await _bot.GetBot();

        var hookUrl = url;

        var path = Route;
        if (!hookUrl.EndsWith(path))
        {
            hookUrl = url.Last() == '/' ? url : url + "/";
            hookUrl += path;
        }
            
        await bot.SetWebhookAsync(hookUrl);

        return Ok();
    }
        
    [HttpGet]
    [Route(WebhookRoute)]
    public async Task<IActionResult> GetWebHook()
    {
        var bot = await _bot.GetBot();

        var webHook = await bot.GetWebhookInfoAsync();

        return Ok(webHook);
    }
    private CultureInfo GetCultureInfo(string languageCode)
    {
        var cultures = new Dictionary<string, string>()
        {
            ["de"] = "de-DE",
            ["en"] = "en-US",
            ["es"] = "es-ES",
            ["ru"] = "ru-RU",
        };

        return cultures.TryGetValue(languageCode, out var cultureCode)
            ? new CultureInfo(cultureCode)
            : CultureInfo.InvariantCulture;
    }
}