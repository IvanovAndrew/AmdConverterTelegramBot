using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

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
    private readonly IRequestParser _requestParser;
    private readonly RateLoader _rateLoader;
    
    private readonly Replies _replies;
    

    public AmdConverterController(ILogger<AmdConverterController> logger, TelegramBot bot, IRequestParser requestParser, RateLoader rateLoader, Replies replies)
    {
        _logger = logger;
        _bot = bot;
        _requestParser = requestParser;
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
        }
            
        else if (_requestParser.TryParseFullRequest(text, out var money, out var cash, out var conversion))
        {
            var loadingResult = await _rateLoader.LoadRates(cash);
            
            if (!loadingResult.IsSuccess)
            {
                await botClient.SendTextMessageAsync(chatId, $"{loadingResult.ErrorMessage}");
            }
            else
            {
                var rates = loadingResult.Value;

                var conversionInfo = rates.Select(ep => ep.Convert(conversion, money)).Where(c => c != null).Select(c => c!);

                IOrderedEnumerable<ConversionInfo> sortedValues; 
                if (conversion.To == Currency.Amd)
                {
                    sortedValues = conversionInfo.OrderByDescending(x => x.Rate.FXRate);
                }
                else
                {
                    sortedValues = conversionInfo.OrderBy(x => x.Rate.FXRate);
                }
                
                var replyTitle = (money.Currency == conversion!.From?
                    $"{money} -> {conversion.To}" : $"{conversion.From} -> {money}") + 
                    $" ({(cash? "Cash" : "Non cash")})";

                CultureInfo cultureInfo = GetCultureInfo(languageCode);
                string replyText = FormatTable(sortedValues, conversion.From == money.Currency? conversion.To : conversion.From, conversionInfo.Count(), cultureInfo);
                
                await botClient.SendTextMessageAsync(chatId, TelegramEscaper.EscapeString($"{replyTitle}```{replyText}```"), ParseMode.MarkdownV2);
            }
        }
        else if (_requestParser.TryParseMoneyAndCash(text, out money, out cash))
        {
            string cashString = cash? "cash" : "non cash";
            string moneyAsString = $"{money.Amount}{money.Currency.Symbol}";
            InlineKeyboardMarkup inlineKeyboard;
            if (money.Currency == Currency.Amd)
            {
                // TODO take values by reflection or from appsettings
                var availableCurrencies = new[] { Currency.Eur, Currency.Usd, Currency.Rur, Currency.GEL };
                inlineKeyboard = new InlineKeyboardMarkup
                (
                    new []
                    {
                        availableCurrencies
                            .Select(c =>
                                InlineKeyboardButton.WithCallbackData(
                                    text: $"{money.Currency.Name}->{c.Name}",
                                    callbackData: $"{cashString} {moneyAsString}->{c.Name}")).ToArray(),
                        availableCurrencies
                            .Select(c =>
                                InlineKeyboardButton.WithCallbackData(
                                    text: $"{c.Name}->{money.Currency.Name}",
                                    callbackData: $"{cashString} {c.Name}->{moneyAsString}"))
                    }
                );
            }
            else
            {
                inlineKeyboard = new InlineKeyboardMarkup(
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text:$"{money.Currency.Name} -> {Currency.Amd.Name}", callbackData: $"{cashString} {moneyAsString}->{Currency.Amd.Name}"), 
                        InlineKeyboardButton.WithCallbackData(text:$"{Currency.Amd.Name} -> {money.Currency.Name}", callbackData: $"{cashString} {Currency.Amd.Name}->{moneyAsString}"), 
                    }
                );
            }
                
            await botClient.SendTextMessageAsync(chatId, text:"What would you like to convert?", replyMarkup: inlineKeyboard);
        }
        else if (_requestParser.TryParseMoney(text, out money))
        {
            string moneyAsString = $"{money.Amount}{money.Currency.Symbol}";
            var inlineKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"Cash",
                        callbackData: $"Cash {moneyAsString}"),
                    InlineKeyboardButton.WithCallbackData(text: $"Non cash",
                        callbackData: $"Non cash {moneyAsString}"),
                });
            await botClient.SendTextMessageAsync(chatId, text:"Cash?", replyMarkup: inlineKeyboard);
        }
        else if (_requestParser.TryParseAmount(text, out var amount))
        {
            var availableCurrencies = Currency.GetAvailableCurrencies();
            var inlineKeyboard = new InlineKeyboardMarkup(
                availableCurrencies.Select
                (
                    currency => InlineKeyboardButton.WithCallbackData(text: currency.Name, callbackData:$"{amount}{currency.Symbol}") 
                ).Chunk(3));
            await botClient.SendTextMessageAsync(chatId, text:"Select currency", replyMarkup: inlineKeyboard);
        }
        else if (_replies.Dialogues.Any(a => a.Message.Contains(text)))
        {
            var a = _replies.Dialogues.First(a => a.Message.Contains(text));

            if (!string.IsNullOrEmpty(a.Reply.Text))
            {
                await botClient.SendTextMessageAsync(chatId, a.Reply.Text, replyToMessageId: messageId);
            }
            else
            {
                await botClient.SendStickerAsync(chatId, new InputOnlineFile(a.Reply.Sticker), replyToMessageId: messageId);
            }
        }
        else 
        {
            await botClient.SendTextMessageAsync(chatId, $"{text} isn't recognized as money");
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

    private string FormatTable(IOrderedEnumerable<ConversionInfo> exchanges, Currency currency, int rowNumber, CultureInfo cultureInfo)
    {
        string[,] rowValues = new string[rowNumber, 3];

        int i = 0;
        foreach (var conversionInfo in exchanges)
        {
            rowValues[i, 0] = conversionInfo.ExchangePoint.Name.Replace("Bank Armenia", "").Replace("Bank (Armenia)", "").Trim();
            rowValues[i, 1] = conversionInfo.Rate != Rate.Unknown? conversionInfo.Rate.FXRate.ToString("0.##", cultureInfo) : "???";

            var money = conversionInfo.FromMoney.Currency == currency
                ? conversionInfo.FromMoney
                : conversionInfo.ToMoney;
            rowValues[i, 2] = conversionInfo.Rate != Rate.Unknown? money.Amount.ToString("0.##", cultureInfo) + $"{currency.Symbol}" : "???";
            i++;
        }

        return MarkdownFormatter.FormatTable(new[] { "Bank", "Rate", exchanges.First().Conversion.ToString() }, rowValues);
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