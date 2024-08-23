using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AmdConverterTelegramBot.Shared;

public class Engine
{
    private readonly RateLoader _rateLoader;
    private readonly TelegramBotClient _bot;
    private readonly CultureInfo _cultureInfo;

    public Engine(RateLoader rateLoader, TelegramBotClient bot, CultureInfo cultureInfo)
    {
        _rateLoader = rateLoader;
        _bot = bot;
        _cultureInfo = cultureInfo;
    }
    
    public async Task HandleRequest(UserRequest request, long chatId)
    {
        if (request is AmountState amountState)
        {
            var enteredAmount = amountState.Amount;
            var availableCurrencies = Currency.GetAvailableCurrencies();
            var inlineKeyboard = new InlineKeyboardMarkup(
                availableCurrencies.Select
                (
                    currency => InlineKeyboardButton.WithCallbackData(text: currency.Name, callbackData:$"{enteredAmount}{currency.Symbol}") 
                ).Chunk(3));
            await _bot.SendTextMessageAsync(chatId, text:"Select currency", replyMarkup: inlineKeyboard);
        }
        else if (request is MoneyState moneyState)
        {
            var money = moneyState.Money;
            string moneyAsString = $"{money.Amount}{money.Currency.Symbol}";
            var inlineKeyboard = new InlineKeyboardMarkup(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "Cash", callbackData: $"cash {moneyAsString}"),
                    InlineKeyboardButton.WithCallbackData(text: "Card", callbackData: $"card {moneyAsString}"),
                });
            await _bot.SendTextMessageAsync(chatId, text:"Cash?", replyMarkup: inlineKeyboard);
        }
        else if (request is MoneyAndWayState moneyAndWayState)
        {
            string cashString = moneyAndWayState.Cash? "cash" : "card";
            var money = moneyAndWayState.Money;
        
            string moneyAsString = $"{money.Amount}{money.Currency.Symbol}";
            InlineKeyboardMarkup inlineKeyboard;
            if (money.Currency == Currency.Amd)
            {
                var availableCurrencies = Currency.GetAvailableCurrencies();
                inlineKeyboard = new InlineKeyboardMarkup
                (
                    new []
                    {
                        availableCurrencies
                            .Where(c => c != Currency.Amd)
                            .Select(c =>
                                InlineKeyboardButton.WithCallbackData(
                                    text: $"{money.Currency.Symbol} -> {c.Symbol}",
                                    callbackData: $"{cashString} {moneyAsString} -> {c.Name}")).ToArray(),
                        availableCurrencies
                            .Where(c => c != Currency.Amd)
                            .Select(c =>
                                InlineKeyboardButton.WithCallbackData(
                                    text: $"{c.Symbol} -> {money.Currency.Symbol}",
                                    callbackData: $"{cashString} {c.Name} -> {moneyAsString}"))
                    }
                );
            }
            else
            {
                inlineKeyboard = new InlineKeyboardMarkup(
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text:$"{money.Currency.Symbol} -> {Currency.Amd.Symbol}", callbackData: $"{cashString} {moneyAsString}->{Currency.Amd.Name}"), 
                        InlineKeyboardButton.WithCallbackData(text:$"{Currency.Amd.Symbol} -> {money.Currency.Symbol}", callbackData: $"{cashString} {Currency.Amd.Name}->{moneyAsString}"), 
                    }
                );
            }
                
            await _bot.SendTextMessageAsync(chatId, text:"What would you like to convert?", replyMarkup: inlineKeyboard);
        }
        else if (request is FullRequest fullRequest)
        {
            bool cash = fullRequest.Cash;
            var loadingResult = await _rateLoader.LoadRates(cash);
            
            if (!loadingResult.IsSuccess)
            {
                await _bot.SendTextMessageAsync(chatId, $"{loadingResult.ErrorMessage}");
            }
            else
            {
                var money = fullRequest.Conversion.Money;
                var conversion = fullRequest.Conversion;
                
                var rates = loadingResult.Value;

                var conversionInfo = 
                    rates
                        .Select(ep => ep.Convert(new Conversion(){From = conversion.From, To = conversion.To}, conversion.Money))
                        .Where(c => c != null)
                        .Select(c => c!);

                IOrderedEnumerable<ConversionInfo> sortedValues; 
                if (fullRequest.Conversion.To == Currency.Amd)
                {
                    sortedValues = conversionInfo.OrderByDescending(x => x.Rate.FXRate);
                }
                else
                {
                    sortedValues = conversionInfo.OrderBy(x => x.Rate.FXRate);
                }

                
                
                var replyTitle = (money.Currency == conversion.From?
                                     $"{money} -> {conversion.To}" : $"{conversion.From} -> {money}") + 
                                 $" ({(cash? "Cash" : "Non cash")})";

                string replyText = FormatTable(sortedValues, conversion.From == money.Currency? conversion.To : conversion.From, conversionInfo.Count(), _cultureInfo);
                
                await _bot.SendTextMessageAsync(chatId, TelegramEscaper.EscapeString($"{replyTitle}```{replyText}```"), parseMode:ParseMode.MarkdownV2);
            }
        }
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

    
}