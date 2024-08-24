using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using System;

namespace RPILogger{
    public class TestLoggerFactory
    {
        public static ILoggerFactory Create(ITestOutputHelper outputHelper)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new XUnitLoggerProvider(outputHelper));
            });
            return loggerFactory;
        }
    }
    public class XUnitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;

        public XUnitLoggerProvider(ITestOutputHelper output)
        {
            _output = output;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XUnitLogger(_output, categoryName);
        }

        public void Dispose() {}

        private class XUnitLogger : ILogger
        {
            private readonly ITestOutputHelper _output;
            private readonly string _categoryName;

            public XUnitLogger(ITestOutputHelper output, string categoryName)
            {
                _output = output;
                _categoryName = categoryName;
            }

            public IDisposable BeginScope<TState>(TState state) => null;
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _output.WriteLine($"{logLevel} - {_categoryName} - {formatter(state, exception)}");
            }
        }
    }

}