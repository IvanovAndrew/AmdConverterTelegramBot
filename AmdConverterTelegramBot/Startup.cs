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
        services.AddSingleton<RateAmOptions>();
        services.AddSingleton<Replies>();
        services.AddScoped<ICurrencyParser, CurrencyParser>(s => ActivatorUtilities.CreateInstance<CurrencyParser>(s, _configuration.GetSection("CurrencySynonyms").GetChildren().ToDictionary(x => x.Key, x => x.Value)));
        services.AddScoped<IMoneyParser, MoneyParser>(s => ActivatorUtilities.CreateInstance<MoneyParser>(s, s.GetRequiredService<ICurrencyParser>(), _configuration["DefaultCurrency"]));
        
        services.AddScoped<IRequestParser, RequestParser>(s => ActivatorUtilities.CreateInstance<RequestParser>(s, s.GetRequiredService<IMoneyParser>(), s.GetRequiredService<ICurrencyParser>(), _configuration.GetSection("Delimiters").Get<string[]>()));
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
