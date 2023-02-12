namespace JayKay.AspNetCore.Logging.Email
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public sealed class EmailLoggerDispatcher : IHostedService, IDisposable
    {
        private readonly IOptions<EmailLoggerConfiguration> config;
        private readonly BlockingCollection<LogMessage> messageQueue;
        private Timer? timer;
        private Task? pollTask;
        private readonly CancellationTokenSource shutdown = new();

        public EmailLoggerDispatcher(IOptions<EmailLoggerConfiguration> config)
        {
            this.config = config;
            messageQueue = new BlockingCollection<LogMessage>(config.Value.MaxQueueSize);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(CheckMessages, null, TimeSpan.FromSeconds(config.Value.PollInterval), Timeout.InfiniteTimeSpan);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Dispose();

            shutdown.Cancel();

            if (pollTask?.IsCompleted ?? true)
            {
                SendMessages();
            }

            return Task.WhenAny(pollTask!, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        public void Dispose()
        {
            timer?.Dispose();
            shutdown?.Dispose();
            messageQueue.Dispose();
            pollTask?.Dispose();
        }

        internal void EnqueueMessage(LogMessage message)
        {
            if (messageQueue.TryAdd(message, 100)) return;

            // queue full
            // block and send messages synchronously

            timer?.Dispose();

            SendMessages();

            if (!shutdown.IsCancellationRequested)
            {
                messageQueue.Add(message);
                SetTimout();
            }
        }

        private void CheckMessages(object? state)
        {
            timer?.Dispose();

            if (shutdown.IsCancellationRequested) return;

            if (messageQueue.Count != 0) pollTask = Task.Run(SendMessages);

            SetTimout();
        }

        private void SendMessages()
        {
            var builder = new StringBuilder();
            int maxItems = config.Value.MaxMessagesPerMail;

            while (maxItems > 0 && messageQueue.TryTake(out LogMessage? message))
            {
                builder.Append($"{message.Timestamp:yyyy-MM-dd} {message.Timestamp:H:mm:ss} {message.LogLevel} - {message.Source}: {message.Message}");
                if (message.Exception != null)
                {
                    builder.AppendLine(message.Exception.ToString());
                }
                builder.AppendLine();
                maxItems--;
            }

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(config.Value.From!),
                Body = builder.ToString(),
                IsBodyHtml = false,
                Subject = config.Value.EmailSubject
            };
            foreach (var to in config.Value.To) mailMessage.To.Add(new MailAddress(to));

            try
            {
                using var smtpClient = new SmtpClient(config.Value.SmtpHost)
                {
                    EnableSsl = config.Value.SmtpUseSSL,
                    Port = config.Value.SmtpPort,
                    Credentials = new NetworkCredential(config.Value.UserName, config.Value.Password)
                };

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private void SetTimout()
        {
            timer = new Timer(CheckMessages, null, TimeSpan.FromSeconds(config.Value.PollInterval), Timeout.InfiniteTimeSpan);
        }
    }
}
