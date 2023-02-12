# AspNetCore Logging to email

This package adds the ability so send log message to an email address.

# Features
- Log messages are queued until a threshold is met
	- Amount of messages
	- Time passed
- Messages are sent with a background task (Hosted Service)

# Setup

In your AspNetCore setup add:

    using JayKay.AspNetCore.Logging.Email;

    public class Program
    {
        public static void Main(string[] args)
        {

            ...

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddEmailLogger();


            ...
        }
    }

# Cofiguration

In your appsettings.json add:

    {
        "Logging": {
        ...
        "Email": {
            "SmtpHost": "[the SMTP host]",
            "UserName": "[the SMTP user name]",
            "Password": "[the SMTP password]",
            "From": "[from address]",
            "To": [ "[to address]", ... ],
            "EmailSubject": "[The email subject]",
            "LogLevel": {
                "Default": ...
            }
        }
    }
    ...

or in your App startup:

    builder.Logging.AddEmailLogger(config =>
    {
        config.EmailSubject = "Error log";
        ...
    });


- PollInterval: Number of seconds to check for new log messages (default: 10)
- MaxMessagesPerMail: Maximum messages per email (default: 50)
- MaxQueueSize: Maximum messages which can be queued until an email is sent synchronously (default: 1024)
- SmtpHost: SMTP server host
- SmtpPort: SMTP server port (default:587)
- SmtpUseSSL: COnnect with SSL/TLS connection (default: false)
- UserName: User name to connect to SMTP server
- Password: Password to connect to SMTP server
- From: Email address to send messages from
- To: List of email addresses to send messages to
- EmailSubject: Email subject line

SmtpHost, UserName, Password, From and at least one non-empty entry in To must be set for the logger to be active.

# License

[License: MIT](https://github.com/jaykay-design/JayKay.AspNetCore.Logging.Email/blob/main/LICENSE.md)

