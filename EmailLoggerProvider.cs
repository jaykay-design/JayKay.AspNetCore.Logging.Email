namespace JayKay.AspNetCore.Logging.Email
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Collections.Concurrent;

    [ProviderAlias("Email")]
    public sealed class EmailLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable? _onChangeToken;
        private EmailLoggerConfiguration _currentConfig;
        private readonly ConcurrentDictionary<string, EmailLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);
        private readonly EmailLoggerDispatcher dispatcher;

        public EmailLoggerProvider(
            IOptionsMonitor<EmailLoggerConfiguration> config,
            EmailLoggerDispatcher dispatcher)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
            this.dispatcher = dispatcher;
        }

        public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(
            categoryName, 
            name => new EmailLogger(name, GetCurrentConfig, dispatcher)
            );
        
        private EmailLoggerConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
