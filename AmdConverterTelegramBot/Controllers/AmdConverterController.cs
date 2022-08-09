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
public class AmdConverterController : ControllerBase
{
    internal const string Route = "api/message/update"; 
    private readonly ILogger _logger;
    private readonly TelegramBot _bot;
    private readonly RateAmService _rateService;
    private readonly IRequestParser _requestParser;
    
    private readonly IReadOnlyDictionary<string, BankInfo> _banksInfo;
    private readonly Replies _replies;

    public AmdConverterController(ILogger<AmdConverterController> logger, TelegramBot bot, IRequestParser requestParser, RateAmService rateService, RateAmOptions rateOptions, Replies replies)
    {
        _logger = logger;
        _bot = bot;
        _requestParser = requestParser;
        _rateService = rateService;
        _banksInfo = rateOptions.Banks;
        _replies = replies;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        var botClient = await _bot.GetBot();

        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            string text = 
                message.Text.Replace(",", ".").ToLowerInvariant().Trim();
            _logger.LogInformation($"{text} is received");
            if (text == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Hello! Enter the price in AMD");
            }
            
            else if (_requestParser.TryParse(text, out var money, out var currency, out var toCurrency))
            {
                if (money != null && currency != null  && toCurrency != null)
                {
                    var exchangesResult = await _rateService.Convert(money, currency, toCurrency.Value);
                    
                    if (!exchangesResult.IsSuccess)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{exchangesResult.ErrorMessage}");
                    }
                    else
                    {
                        string replyText = FormatTable(exchangesResult.Value, currency, money, toCurrency.Value);

                        await botClient.SendTextMessageAsync(message.Chat.Id, $"```{replyText}```", ParseMode.MarkdownV2);
                    }
                }
                else
                {
                    if (money != null)
                    {
                        string moneyAsString = $"{money.Amount}{money.Currency.Symbol}";
                        InlineKeyboardMarkup inlineKeyboard;
                        if (money.Currency == Currency.Amd)
                        {
                            var availableCurrencies = new[] { Currency.Eur, Currency.Usd, Currency.Rur };
                            inlineKeyboard = new InlineKeyboardMarkup
                            (
                                new []
                                {
                                    availableCurrencies
                                        .Select(c =>
                                            InlineKeyboardButton.WithCallbackData(
                                                text: $"{money.Currency.Name} -> {c.Name}",
                                                callbackData: $"{moneyAsString}->{c.Name}")).ToArray(),
                                    availableCurrencies
                                        .Select(c =>
                                            InlineKeyboardButton.WithCallbackData(
                                                text: $"{c.Name} -> {money.Currency.Name}",
                                                callbackData: $"{c.Name}->{moneyAsString}"))
                                }
                            );
                        }
                        else
                        {
                            inlineKeyboard = new InlineKeyboardMarkup(
                                new []
                                {
                                    InlineKeyboardButton.WithCallbackData(text:$"{money.Currency.Name} -> {Currency.Amd.Name}", callbackData:$"{moneyAsString}->{Currency.Amd.Name}"), 
                                    InlineKeyboardButton.WithCallbackData(text:$"{Currency.Amd.Name} -> {money.Currency.Name}", callbackData:$"{Currency.Amd.Name}->{moneyAsString}"), 
                                }
                            );
                        }
                        
                        
                        await botClient.SendTextMessageAsync(message.Chat.Id, text:"What should I do?", replyMarkup: inlineKeyboard);
                    }
                }
            }
            else if (_replies.Dialogues.Any(a => a.Message.Contains(text)))
            {
                var a = _replies.Dialogues.First(a => a.Message.Contains(text));

                if (!string.IsNullOrEmpty(a.Reply.Text))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, a.Reply.Text, replyToMessageId: message.MessageId);
                }
                else
                {
                    await botClient.SendStickerAsync(message.Chat.Id, new InputOnlineFile(a.Reply.Sticker), replyToMessageId: message.MessageId);
                }
            }
            else 
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"{message.Text} isn't recognized as number");
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            string text = update.CallbackQuery.Data;
            var message = update.CallbackQuery.Message;
            if (_requestParser.TryParse(text, out var money, out var currency, out var toCurrency))
            {
                if (money != null && currency != null  && toCurrency != null)
                {
                    var exchangesResult = await _rateService.Convert(money, currency, toCurrency.Value);
                    
                    if (!exchangesResult.IsSuccess)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"{exchangesResult.ErrorMessage}");
                    }
                    else
                    {
                        string replyText = FormatTable(exchangesResult.Value, currency, money, toCurrency.Value);

                        await botClient.SendTextMessageAsync(message.Chat.Id, $"```{replyText}```", ParseMode.MarkdownV2);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Something wrong with buttons");
                }
            }
        }

        return Ok();
    }

    private string FormatTable(Dictionary<Bank, decimal> exchanges, Currency currency, Money money, bool toCurrency)
    {
        string[,] rowValues = new string[exchanges.Count, 3];

        int i = 0;
        foreach (var (bank, values) in exchanges.OrderBy(kvp => kvp.Value))
        {
            var rate = bank.Rates[currency != Currency.Amd? currency : money.Currency];

            decimal? usedRate = null;
            if (rate != Rate.Unknown)
            {
                if (toCurrency)
                {
                    usedRate = money.Currency == Currency.Amd ? rate.Sell : rate.Buy;
                }
                else
                {
                    usedRate = money.Currency == Currency.Amd ? rate.Buy : rate.Sell;
                }
            }
            
            _banksInfo.TryGetValue(bank.Name.ToLowerInvariant(), out BankInfo bankInfo);
            
            rowValues[i, 0] = bankInfo?.Alias?? bank.Name;
            rowValues[i, 1] = usedRate != null? usedRate.Value.ToString()  : "???";
            rowValues[i, 2] = rate != Rate.Unknown? values.ToString("0.##") + $" {currency.Symbol}" : "???";
            i++;
        }

        var lastColumnName = toCurrency
            ? $"{money} -> {currency.Symbol}"
            : $"{currency.Symbol} -> {money}"; 

        return MarkdownFormatter.FormatTable(new[] { "Bank", "Rate", lastColumnName }, rowValues);
    }
}