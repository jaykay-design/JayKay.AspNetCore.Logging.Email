namespace JayKay.AspNetCore.Logging.Email
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Configuration;

    public static class EmailLoggerExtension
    {
        public static ILoggingBuilder AddEmailLogger(
            this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.AddSingleton<ILoggerProvider, EmailLoggerProvider>();
            LoggerProviderOptions.RegisterProviderOptions<EmailLoggerConfiguration, EmailLoggerProvider>(builder.Services);

            builder.Services.AddSingleton<EmailLoggerDispatcher>();
            builder.Services.AddSingleton<IHostedService>(x => x.GetRequiredService<EmailLoggerDispatcher>());

            return builder;
        }

        public static ILoggingBuilder AddEmailLogger(
            this ILoggingBuilder builder,
            Action<EmailLoggerConfiguration> configure)
        {
            builder.AddConfiguration();

            builder.Services.AddSingleton<ILoggerProvider, EmailLoggerProvider>();
            LoggerProviderOptions.RegisterProviderOptions<EmailLoggerConfiguration, EmailLoggerProvider>(builder.Services);

            builder.Services.Configure(configure);

            builder.Services.AddSingleton<EmailLoggerDispatcher>();
            builder.Services.AddSingleton<IHostedService>(x => x.GetRequiredService<EmailLoggerDispatcher>());

            return builder;
        }
    }
}
