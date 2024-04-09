using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Entities;
using AmdConverterTelegramBot.Shared.Services;
using AmdConverterTelegramBot.Shared.SiteParser;
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
        services.AddLogging();
        
        var builder = services.AddControllers().AddNewtonsoftJson();
        
        
        var cultureInfo = new CultureInfo(_configuration["CultureInfo"]);
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Warrior", Version = "v1" });
        });
        services.AddSingleton<Shared.Services.TelegramBot>();
        services.AddSingleton<CurrencyParser>();
        
        services.AddSingleton<IMoneyParser, MoneyParser>(s => ActivatorUtilities.CreateInstance<MoneyParser>(s, s.GetRequiredService<CurrencyParser>()));
        services.AddSingleton<IBankParserFactory, BankParserFactory>(s => ActivatorUtilities.CreateInstance<BankParserFactory>(s, s.GetRequiredService<CurrencyParser>(), cultureInfo));
        
        services.AddScoped<RateAmParser>(s =>
            ActivatorUtilities.CreateInstance<RateAmParser>(s, s.GetRequiredService<CurrencyParser>(), cultureInfo));
        services.AddScoped<RateLoader>(s => ActivatorUtilities.CreateInstance<RateLoader>(s, 
            s.GetRequiredService<IBankParserFactory>(), 
            s.GetRequiredService<RateAmParser>(), 
            s.GetService<ILoggerFactory>().CreateLogger<RateLoader>()));
        
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
