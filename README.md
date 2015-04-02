# Serilog.Sinks.Email

[![Build status](https://ci.appveyor.com/api/projects/status/sfvp7dw8u6aiodj1/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-email/branch/master)

Sends log events by SMTP email.

**Package** - [Serilog.Sinks.Email](http://nuget.org/packages/serilog.sinks.email)
| **Platforms** - .NET 4.5

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Email(
        from: "app@example.com",
        to: "support@example.com",
        mailServer: "smtp.example.com")
    .CreateLogger();
```

An overload accepting `EmailConnectionInfo` can be used to specify advanced options.
