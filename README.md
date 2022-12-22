# Serilog.Sinks.Email

[![Build status](https://ci.appveyor.com/api/projects/status/sfvp7dw8u6aiodj1/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-email/branch/master)

Sends log events by SMTP email.

**Package** - [Serilog.Sinks.Email](http://nuget.org/packages/serilog.sinks.email)
| **Platforms** - .NET 4.5

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Email(
        fromEmail: "app@example.com",
        toEmail: "support@example.com",
        mailServer: "smtp.example.com")
    .CreateLogger();
```

An overload accepting `EmailConnectionInfo` can be used to specify advanced options.

Other types of email transport can be also used like SqlServer sp_send_dbmail
```csharp
var log = new LoggerConfiguration()
    .WriteTo.Email(ńew SqlServerEmailConnectionInfo
    {
        ToEmail = "support@example.com",
        MailProfileName = "My_Profile",
        SqlConnectionString = "Data Source=(local);..."
    })
    .CreateLogger();
```

You can also use appsettings.json when Serilog.Configuration is used combined with filters to send only exceptions eg. on environments where monitoring is limited.
```json
 "Serilog": {
        "Using": [ "Serilog" ],
        "WriteTo": [
            {
                "Name": "Email",
                "Filter": [
                    {
                        "Name": "ByExcluding",
                        "Args": {
                            "expression": "@Level <> Error"
                        }
                    }
                ],
                "Args": {
                    "sqlConnectionString": "Data Source=(local);...",
                    "mailProfileName": "My_Profile",
                    "toEmail": "support@example.com",
                    "emailSubject": "Exception on PROD"
                }
            }
        ]
    }
```
