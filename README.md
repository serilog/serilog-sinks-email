# Serilog.Sinks.Email

[![Build status](https://ci.appveyor.com/api/projects/status/sfvp7dw8u6aiodj1/branch/main?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-email/branch/main)

Sends log events by SMTP email.

**Package** - [Serilog.Sinks.Email](http://nuget.org/packages/serilog.sinks.email)

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Email(
        from: "app@example.com",
        to: "support@example.com",
        host: "smtp.example.com")
    .CreateLogger();
```

Supported options are:

| Parameter              | Description                                                                                                                        |
|------------------------|------------------------------------------------------------------------------------------------------------------------------------|
| `from`                 | The email address emails will be sent from.                                                                                        |
| `to`                   | The email address emails will be sent to. Multiple addresses can be separated with commas or semicolons.                           |
| `host`                 | The SMTP server to use.                                                                                                            |
| `port`                 | The port used for the SMTP connection. The default is 25.                                                                          |
| `connectionSecurity`   | Choose the security applied to the SMTP connection. This enumeration type is supplied by MailKit. The default is `Auto`.           |
| `credentials`          | The network credentials to use to authenticate with the mail server.                                                               |
| `subject`              | A message template describing the email subject. The default is `"Log Messages"`.                                                  |
| `body`                 | A message template describing the format of the email body. The default is `"{Timestamp} [{Level}] {Message}{NewLine}{Exception}"`. |
| `formatProvider`       | Supplies culture-specific formatting information. The default is to use the current culture.                                       |

An overload accepting `EmailSinkOptions` can be used to specify advanced options such as HTML body templates.
