using System.Globalization;
using AmdConverterTelegramBot.Entities;
using AmdConverterTelegramBot.Services;
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
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Warrior", Version = "v1" });
        });
        services.AddSingleton<TelegramBot>();
        services.AddSingleton<RateSources>();
        
        
        services.AddScoped<ICurrencyParser, CurrencyParser>(s => ActivatorUtilities.CreateInstance<CurrencyParser>(s, _configuration.GetSection("CurrencySynonyms").GetChildren().ToDictionary(x => x.Key, x => x.Value)));
        services.AddScoped<IMoneyParser, MoneyParser>(s => ActivatorUtilities.CreateInstance<MoneyParser>(s, s.GetRequiredService<ICurrencyParser>(), _configuration["DefaultCurrency"]));

        var cultureInfo = new CultureInfo(_configuration["CultureInfo"]);
        services.AddScoped<SasSiteParser>(s => ActivatorUtilities.CreateInstance<SasSiteParser>(s, s.GetRequiredService<ICurrencyParser>(), cultureInfo));
        services.AddScoped<MirSiteParser>(s => ActivatorUtilities.CreateInstance<MirSiteParser>(s));
        services.AddScoped<RateAmParser>(s =>
            ActivatorUtilities.CreateInstance<RateAmParser>(s, s.GetRequiredService<IMoneyParser>(), cultureInfo));
        services.AddScoped<RateLoader>(s => ActivatorUtilities.CreateInstance<RateLoader>(s, s.GetRequiredService<RateAmParser>()));
        
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
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AMD to RUR converter API"));
        serviceProvider.GetRequiredService<TelegramBot>().GetBot().Wait();
        
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
