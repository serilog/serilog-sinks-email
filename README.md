# Serilog.Sinks.Email

[![Build status](https://ci.appveyor.com/api/projects/status/sfvp7dw8u6aiodj1/branch/main?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-email/branch/main)

Sends log events by SMTP email.

**Package** - [Serilog.Sinks.Email](http://nuget.org/packages/serilog.sinks.email)

```csharp
await using var log = new LoggerConfiguration()
    .WriteTo.Email(
        from: "app@example.com",
        to: "support@example.com",
        host: "smtp.example.com")
    .CreateLogger();
```

Supported options are:

| Parameter              | Description                                                                                                                         |
|------------------------|-------------------------------------------------------------------------------------------------------------------------------------|
| `from`                 | The email address emails will be sent from.                                                                                         |
| `to`                   | The email address emails will be sent to. Multiple addresses can be separated with commas or semicolons.                            |
| `host`                 | The SMTP server to use.                                                                                                             |
| `port`                 | The port used for the SMTP connection. The default is 25.                                                                           |
| `connectionSecurity`   | Choose the security applied to the SMTP connection. This enumeration type is supplied by MailKit. The default is `Auto`.            |
| `credentials`          | The network credentials to use to authenticate with the mail server.                                                                |
| `subject`              | A message template describing the email subject. The default is `"Log Messages"`.                                                   |
| `body`                 | A message template describing the format of the email body. The default is `"{Timestamp} [{Level}] {Message}{NewLine}{Exception}"`. |
| `formatProvider`       | Supplies culture-specific formatting information. The default is to use the current culture.                                        |

An overload accepting `EmailSinkOptions` can be used to specify advanced options such as batched and/or HTML body templates.

## Sending batch email

To send batch email, supply `WriteTo.Email` with a batch size:

```csharp
await using var log = new LoggerConfiguration()
    .WriteTo.Email(
        options: new()
        {
            From = "app@example.com",
            To = "support@example.com",
            Host = "smtp.example.com",
        },
        batchingOptions: new()
        {
            BatchSizeLimit = 10,
            Period = TimeSpan.FromSeconds(30),
        })
    .CreateLogger();
```

Batch formatting can be customized using `options.Body`.

## Sending HTML email

To send HTML email, specify a custom `IBatchTextFormatter` in `options.Body` and set `options.IsBodyHtml` to `true`:


```csharp
await using var log = new LoggerConfiguration()
    .WriteTo.Email(
        options: new()
        {
            From = "app@example.com",
            To = "support@example.com",
            Host = "smtp.example.com",
            Body = new MyHtmlBodyFormatter(),
            IsBodyHtml = true,
        },
        batchingOptions: new()
        {
            BatchSizeLimit = 10,
            Period = TimeSpan.FromSeconds(30),
        })
    .CreateLogger();
```

A simplistic HTML formatter is shown below:

```csharp
class MyHtmlBodyFormatter : IBatchTextFormatter
{
    public void FormatBatch(IEnumerable<LogEvent> logEvents, TextWriter output)
    {
        output.Write("<table>");
        foreach (var logEvent in logEvents)
        {
            output.Write("<tr>");
            Format(logEvent, output);
            output.Write("</tr>");
        }

        output.Write("</table>");
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        using var buffer = new StringWriter();
        logEvent.RenderMessage(buffer);
        output.Write(WebUtility.HtmlEncode(buffer.ToString()));
    }
}
```
