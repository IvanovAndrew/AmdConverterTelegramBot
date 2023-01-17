using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
using AmdConverterTelegramBot.SiteParser;
using Microsoft.OpenApi.Models;

namespace AmdConverterTelegramBot;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var builder = services.AddControllers().AddNewtonsoftJson();
        
        var cultureInfo = new CultureInfo(_configuration["CultureInfo"]);
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Warrior", Version = "v1" });
        });
        services.AddSingleton<TelegramBot>();
        services.AddSingleton<RateSources>();
        
        services.AddSingleton<ICurrencyParser, CurrencyParser>(s => ActivatorUtilities.CreateInstance<CurrencyParser>(s, _configuration.GetSection("CurrencySynonyms").GetChildren().ToDictionary(x => x.Key, x => x.Value)));
        services.AddSingleton<IMoneyParser, MoneyParser>(s => ActivatorUtilities.CreateInstance<MoneyParser>(s, s.GetRequiredService<ICurrencyParser>(), _configuration["DefaultCurrency"]));
        services.AddSingleton<IBankParserFactory, BankParserFactory>(s => ActivatorUtilities.CreateInstance<BankParserFactory>(s, s.GetRequiredService<ICurrencyParser>(), cultureInfo));
        
        services.AddScoped<RateAmParser>(s =>
            ActivatorUtilities.CreateInstance<RateAmParser>(s, s.GetRequiredService<IMoneyParser>(), cultureInfo));
        services.AddScoped<RateLoader>(s => ActivatorUtilities.CreateInstance<RateLoader>(s, s.GetRequiredService<IBankParserFactory>(), s.GetRequiredService<RateAmParser>(), s.GetRequiredService<RateSources>()));
        
        services.AddScoped<IRequestParser, RequestParser>(s => ActivatorUtilities.CreateInstance<RequestParser>(s, s.GetRequiredService<IMoneyParser>(), s.GetRequiredService<ICurrencyParser>(), _configuration.GetSection("Delimiters").Get<string[]>()));
        
        services.AddSingleton<Replies>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AMD converter API"));
        serviceProvider.GetRequiredService<TelegramBot>().GetBot().Wait();
        
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
