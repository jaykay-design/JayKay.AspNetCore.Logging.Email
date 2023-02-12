using Microsoft.Extensions.Logging;

namespace JayKay.AspNetCore.Logging.Email
{
    internal sealed class LogMessage
    {
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = string.Empty;
        internal LogLevel LogLevel { get; set; }
        internal EventId EventId { get; set; }
        internal Exception? Exception { get; set; }
        internal string Message { get; set; } = string.Empty;
    }
}
