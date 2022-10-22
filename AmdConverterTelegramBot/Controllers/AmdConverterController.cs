using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly IRequestParser _requestParser;
    
    private readonly RateAmOptions _rateAmOptions;
    private readonly Replies _replies;

    public AmdConverterController(ILogger<AmdConverterController> logger, TelegramBot bot, IRequestParser requestParser, RateAmOptions rateOptions, Replies replies)
    {
        _logger = logger;
        _bot = bot;
        _requestParser = requestParser;
        _rateAmOptions = rateOptions;
        _replies = replies;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        var botClient = await _bot.GetBot();

        long chatId = 0;
        int messageId = 0;
        string text = string.Empty;
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
        }

        _logger.LogInformation($"{text} is received");
        
        
        if (text == "/start")
        {
            await botClient.SendTextMessageAsync(chatId, "Hello! Enter the price in AMD, USD, EUR, or RUB");
        }
            
        else if (_requestParser.TryParse(text, out var money, out var cash, out var currency, out var toCurrency))
        {
            if (money != null && cash != null && currency != null && toCurrency != null)
            {
                var rateService = new RateAmService(cash.Value ? _rateAmOptions.CashUrl : _rateAmOptions.NonCashUrl);
                var exchangesResult = await rateService.Convert(money, currency, toCurrency.Value);
                
                if (!exchangesResult.IsSuccess)
                {
                    await botClient.SendTextMessageAsync(chatId, $"{exchangesResult.ErrorMessage}");
                }
                else
                {
                    string replyText = FormatTable(exchangesResult.Value, currency, money, toCurrency.Value);
                    await botClient.SendTextMessageAsync(chatId, $"{(cash.Value? "Cash" : "Non cash")}{Environment.NewLine}```{replyText}```", ParseMode.MarkdownV2);
                }
            }
            else if (money != null && cash != null)
            {
                string cashString = cash.Value ? "cash" : "non cash";
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
                                        callbackData: $"{cashString} {moneyAsString}->{c.Name}")).ToArray(),
                            availableCurrencies
                                .Select(c =>
                                    InlineKeyboardButton.WithCallbackData(
                                        text: $"{c.Name} -> {money.Currency.Name}",
                                        callbackData: $"{cashString} {c.Name}->{moneyAsString}"))
                        }
                    );
                }
                else
                {
                    inlineKeyboard = new InlineKeyboardMarkup(
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text:$"{money.Currency.Name} -> {Currency.Amd.Name}", callbackData:$"{cashString} {moneyAsString}->{Currency.Amd.Name}"), 
                            InlineKeyboardButton.WithCallbackData(text:$"{Currency.Amd.Name} -> {money.Currency.Name}", callbackData:$"{cashString} {Currency.Amd.Name}->{moneyAsString}"), 
                        }
                    );
                }
                
                await botClient.SendTextMessageAsync(chatId, text:"What would you like to convert?", replyMarkup: inlineKeyboard);
            }
            else if (money != null)
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
            
            _rateAmOptions.Banks.TryGetValue(bank.Name.ToLowerInvariant(), out BankInfo bankInfo);
            
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