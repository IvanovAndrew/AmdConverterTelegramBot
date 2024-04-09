using System.Collections.Generic;
using System.Globalization;
using System.Net;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.Services;
using AmdConverterTelegramBot.Shared.SiteParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sprache;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AzureFunction;

public class Function
{
    private readonly ILogger _logger;

    public Function(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Function>();
    }

    [Function("AmdConverterAzureFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        Update update = JsonConvert.DeserializeObject<Update>(requestBody);

        var token = Environment.GetEnvironmentVariable("token", EnvironmentVariableTarget.Process);

        if (token is null)
        {
            throw new NullReferenceException("Missing telegram bot token");
        }
        else
        {
            _logger.LogInformation("Token has been read");
        }
        
        var bot = new TelegramBot(token);
        var botClient = await bot.GetBot();

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
        
        CultureInfo cultureInfo = GetCultureInfo(languageCode);

        text = TelegramEscaper.Decode(text);

        _logger.LogInformation($"{text} is received");

        var rateLoader = new RateLoader(new BankParserFactory(new CurrencyParser(), cultureInfo), new RateAmLoader(), _logger);

        if (text == "/start")
        {
            await botClient.SendTextMessageAsync(chatId, "Hello! Enter the price in AMD, USD, EUR, GEL, or RUB");
        }
        else
        {
            var request = Grammar.Request.Parse(text);
            var engine = new Engine(rateLoader, botClient, GetCultureInfo(languageCode));

            await engine.HandleRequest(request, chatId);
        }
        
        return CreateResponse(req);
    }

    private static HttpResponseData CreateResponse(HttpRequestData request)
    {
        var response = request.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");

        return response;
    }

    private static CultureInfo GetCultureInfo(string languageCode)
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
    
    private static string FormatTable(IOrderedEnumerable<ConversionInfo> exchanges, Currency currency, int rowNumber, CultureInfo cultureInfo)
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