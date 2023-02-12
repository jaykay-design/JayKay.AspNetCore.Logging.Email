namespace JayKay.AspNetCore.Logging.Email
{
    using Microsoft.Extensions.Logging;
    using System;

    public sealed class EmailLogger : ILogger
    {
        private readonly string name;
        private readonly Func<EmailLoggerConfiguration> getCurrentConfig;
        private readonly EmailLoggerDispatcher dispatcher;

        public EmailLogger(
            string name,
            Func<EmailLoggerConfiguration> getCurrentConfig,
            EmailLoggerDispatcher dispatcher)
        {
            this.name = name;
            this.getCurrentConfig = getCurrentConfig;
            this.dispatcher = dispatcher;
        }


        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => getCurrentConfig().IsConfigured;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            dispatcher.EnqueueMessage(new LogMessage()
            {
                Source = this.name,
                LogLevel = logLevel,
                EventId = eventId,
                Exception = exception,
                Message = formatter(state, exception),
                Timestamp = DateTime.Now
            });
        }
    }
}
