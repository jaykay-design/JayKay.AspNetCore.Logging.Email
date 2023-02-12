using System.Net.Mail;

namespace JayKay.AspNetCore.Logging.Email
{
    public sealed class EmailLoggerConfiguration
    {
        public bool IsConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SmtpHost)
                        && !string.IsNullOrWhiteSpace(UserName)
                        && !string.IsNullOrWhiteSpace(Password)
                        && !string.IsNullOrWhiteSpace(From)
                        && To.Length != 0
                        && To.All(t => !string.IsNullOrWhiteSpace(t));
            }
        }

        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; } = 587;
        public bool SmtpUseSSL{ get; set; } = false;
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? From { get; set; }
        public string[] To { get; set; } = Array.Empty<string>();
        public string EmailSubject { get; set; } = "Email logging error";

        public int PollInterval { get; set; } = 10;
        public int MaxMessagesPerMail { get; set; } = 50;
        public int MaxQueueSize { get; set; } = 256;
    }
}