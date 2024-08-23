using System.Globalization;
using AmdConverterTelegramBot.Shared;
using AmdConverterTelegramBot.Shared.Services;
using AmdConverterTelegramBot.Shared.SiteParser;
using Microsoft.Extensions.Logging;
using Xunit;
using Assert = Xunit.Assert;

namespace SiteParsersTests;

public class RateLoaderTest
{
    internal class StubLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }
    }
}