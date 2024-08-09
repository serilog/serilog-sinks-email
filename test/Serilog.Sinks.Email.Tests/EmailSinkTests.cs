using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Configuration;
using Serilog.Sinks.Email.Tests.Support;
using Xunit;
using Xunit.Abstractions;
using System.Net;

namespace Serilog.Sinks.Email.Tests;

public class EmailSinkTests
{
    public EmailSinkTests(ITestOutputHelper outputHelper)
    {
        SelfLog.Enable(outputHelper.WriteLine);
    }

    [Fact]
    public void Works()
    {
        // Disable certificate check for internal self-signed certificates
        ServicePointManager.ServerCertificateValidationCallback =
            (sender, certificate, chain, sslPolicyErrors) => true;

        var selfLogMessages = new List<string>();
        SelfLog.Enable(selfLogMessages.Add);

        using (var emailLogger = new LoggerConfiguration()
                   .WriteTo.Email(
                       from: "noreply@amatest.net",
                       to: "admrouzies@amatest.net",
                       host: "smtp.amatest.net",
                       body: "[{Level}] {Message}{NewLine}{Exception} <b>bold</b> <img src='' alt='Alt text'/>",
                       subject: "subject",
                       port: 587,
                       useDefaultCredentials: true,
                       isBodyHtml: true)
                   .CreateLogger())
        {
            emailLogger.Information("test {test}", "test");
        }

        Assert.Equal(Enumerable.Empty<string>(), selfLogMessages);
    }

    [Fact(Skip = "Requires a smtp mail server")]
    public void WorksMultipleEventsInOneMail()
    {
        var selfLogMessages = new List<string>();
        SelfLog.Enable(selfLogMessages.Add);

        using (var emailLogger = new LoggerConfiguration()
                   .WriteTo.Email(
                       from: "from@smtpserver.local",
                       to: "to@smtpserver.local",
                       host: "smtpserver.local",
                       body: "[{Level}] {Message}{NewLine}{Exception}",
                       subject: "test subject")
                   .CreateLogger())
        {
            emailLogger.Information("first test {test}", "test1");
            emailLogger.Error("second {test}", "test2");
            emailLogger.Fatal("third {test}", "test3");
        }

        Assert.Equal(Enumerable.Empty<string>(), selfLogMessages);
    }

    [Fact]
    public void EmailTransportIsDisposedWhenEmailSinkIsDisposed()
    {
        var transport = new TestEmailTransport();
        var emailSink = CreateDefaultEmailSink(new EmailSinkOptions(), transport);

        emailSink.Dispose();

        Assert.True(transport.IsDisposed);
    }

    [Fact]
    [UseCulture("en-us")]
    public async Task SendEmailIsCorrectlyCalledWhenEventAreLogged()
    {
        var emailConnectionInfo = new EmailSinkOptions
        {
            To = ["to@localhost.local"],
            From = "from@localhost.local",
            Body = new MessageTemplateTextFormatter("[{Level}] {Message}{NewLine}{Exception}"),
            Subject = new MessageTemplateTextFormatter("[{Level}] A message")
        };

        var transport = new TestEmailTransport();

        var emailSink = CreateDefaultEmailSink(emailConnectionInfo, transport);

        await emailSink.EmitBatchAsync(new[]
        {
            new LogEvent(
                DateTimeOffset.Now,
                LogEventLevel.Error,
                // ReSharper disable once NotResolvedInText
                new ArgumentOutOfRangeException("parameter1", "Message of the exception"),
                new MessageTemplate("Subject",
                    new MessageTemplateToken[]
                    {
                        new PropertyToken("Message", "A multiline" + Environment.NewLine + "Message")
                    })
                , Enumerable.Empty<LogEventProperty>())
        });
        emailSink.Dispose();

        var actual = transport.Sent.Single();

        Assert.Equal("[Error] A multiline" + Environment.NewLine
                                           + "Message" + Environment.NewLine
                                           + "System.ArgumentOutOfRangeException: Message of the exception"
                                           + " (Parameter 'parameter1')"
                                           + Environment.NewLine + "", actual.Body);
        Assert.Equal("[Error] A message", actual.Subject);
        Assert.Equal("from@localhost.local", actual.From);
        Assert.Equal(new[] { "to@localhost.local" }, actual.To);
        Assert.False(actual.IsBodyHtml);
    }

    [Fact]
    public void MultilineMessageCreatesSubjectWithTheFirstLineOnly()
    {
        var subjectLineFormatter = new MessageTemplateTextFormatter("{Message}", null);

        var logEvents = new[]
        {
            new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, new Exception("An exception occured"),
                new MessageTemplate(@"Subject",
                    new MessageTemplateToken[]{new PropertyToken("Message", "A multiline" + Environment.NewLine + "Message")})
                , Enumerable.Empty<LogEventProperty>())
        };
        var mailSubject = EmailSink.ComputeMailSubject(subjectLineFormatter, logEvents);

        Assert.Equal("A multiline", mailSubject);
    }

    [Fact]
    public void WorksWithIBatchTextFormatter()
    {
        var emailConnectionInfo = new EmailSinkOptions
        {
            To = ["to@example.com"],
            From = "from@localhost.local",
            IsBodyHtml = true,
            Body = new HtmlTableFormatter()
        };

        var emailTransport = new TestEmailTransport();
        var sink = new EmailSink(emailConnectionInfo, emailTransport);

        using (var emailLogger = new LoggerConfiguration()
                   .WriteTo.Sink(sink, new BatchingOptions())
                   .CreateLogger())
        {
            emailLogger.Information("Information");
            emailLogger.Warning("Warning");
            emailLogger.Error("<Error>");
        }

        var single = emailTransport.Sent.Single();
        Assert.True(single.IsBodyHtml);
        Assert.Equal("<table><tr>Information</tr><tr>Warning</tr><tr>&lt;Error&gt;</tr></table>", single.Body);
    }

    static EmailSink CreateDefaultEmailSink(EmailSinkOptions options, IEmailTransport transport)
    {
        var emailSink = new EmailSink(
            options,
            transport);
        return emailSink;
    }
}
