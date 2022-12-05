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
public class AmdConverterController : ControllerBase
{
    internal const string Route = "api/message/update"; 
    private readonly ILogger _logger;
    private readonly TelegramBot _bot;
    private readonly IRequestParser _requestParser;
    private readonly Parser _parser;
    
    private readonly Replies _replies;
    

    public AmdConverterController(ILogger<AmdConverterController> logger, TelegramBot bot, IRequestParser requestParser, Parser parser, Replies replies)
    {
        _logger = logger;
        _bot = bot;
        _requestParser = requestParser;
        _parser = parser;
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

        text = TelegramEscaper.Decode(text);

        _logger.LogInformation($"{text} is received");
        
        
        if (text == "/start")
        {
            await botClient.SendTextMessageAsync(chatId, "Hello! Enter the price in AMD, USD, EUR, or RUB");
        }
            
        else if (_requestParser.TryParseFullRequest(text, out var money, out var cash, out var conversion))
        {
            var loadingResult = await (cash? _parser.LoadCashRates() : _parser.LoadNonCashRates());
            
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
                    sortedValues = conversionInfo.OrderByDescending(x => x.ToMoney.Amount);
                }
                else
                {
                    sortedValues = conversionInfo.OrderBy(x => x.FromMoney.Amount);
                }
                
                var replyTitle = (money.Currency == conversion!.From?
                    $"{money} -> {conversion.To}" : $"{conversion.From} -> {money}") + 
                    $" ({(cash? "Cash" : "Non cash")})";
                
                string replyText = FormatTable(sortedValues, conversion.From == money.Currency? conversion.To : conversion.From, conversionInfo.Count());
                
                await botClient.SendTextMessageAsync(chatId, TelegramEscaper.EscapeString($"{replyTitle}{Environment.NewLine}```{replyText}```"), ParseMode.MarkdownV2);
            }
        }
        else if (_requestParser.TryParseMoneyAndCash(text, out money, out cash))
        {
            string cashString = cash? "cash" : "non cash";
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

    private string FormatTable(IOrderedEnumerable<ConversionInfo> exchanges, Currency currency, int rowNumber)
    {
        string[,] rowValues = new string[rowNumber, 3];

        int i = 0;
        foreach (var conversionInfo in exchanges)
        {
            rowValues[i, 0] = conversionInfo.ExchangePoint.Name.Replace("Bank Armenia", "").Replace("Bank (Armenia)", "").Trim();
            rowValues[i, 1] = conversionInfo.Rate != Rate.Unknown? conversionInfo.Rate.FXRate.ToString("0.##", CultureInfo.InvariantCulture) : "???";

            var money = conversionInfo.FromMoney.Currency == currency
                ? conversionInfo.FromMoney
                : conversionInfo.ToMoney;
            rowValues[i, 2] = conversionInfo.Rate != Rate.Unknown? money.Amount.ToString("0.##") + $"{currency.Symbol}" : "???";
            i++;
        }

        return MarkdownFormatter.FormatTable(new[] { "Bank", "Rate", exchanges.First().Conversion.ToString() }, rowValues);
    }
}