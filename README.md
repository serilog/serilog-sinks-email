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
    .CreateLogger();~~~~
```

An overload accepting `EmailSinkOptions` can be used to specify advanced options.
