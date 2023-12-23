using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Sinks.PeriodicBatching;
using Xunit;
using Xunit.Abstractions;

namespace Serilog.Sinks.Email.Tests;

public class EmailSinkTests
{
    public EmailSinkTests(ITestOutputHelper outputHelper)
    {
        SelfLog.Enable(outputHelper.WriteLine);
    }

    [Fact(Skip = "Requires a localhost mail server")]
    public void Works()
    {
        var selfLogMessages = new List<string>();
        SelfLog.Enable(selfLogMessages.Add);

        using (var emailLogger = new LoggerConfiguration()
                   .WriteTo.Email(
                       fromEmail: "from@localhost.local",
                       toEmail: "to@localhost.local",
                       mailServer: "localhost",
                       outputTemplate: "[{Level}] {Message}{NewLine}{Exception}",
                       mailSubject: "subject")
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
                       fromEmail: "from@smtpserver.local",
                       toEmail: "to@smtpserver.local",
                       mailServer: "smtpserver.local",
                       outputTemplate: "[{Level}] {Message}{NewLine}{Exception}",
                       mailSubject: "test subject")
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
        var emailSink = CreateDefaultEmailSink(new EmailConnectionInfo(), transport);

        emailSink.Dispose();

        Assert.True(transport.IsDisposed);
    }

    [Fact]
    [UseCulture("en-us")]
    public async Task SendEmailIsCorrectlyCalledWhenEventAreLogged()
    {
        var emailConnectionInfo = new EmailConnectionInfo
        {
            ToEmail = "to@localhost.local",
            FromEmail = "from@localhost.local",
            IsBodyHtml = true
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
                new MessageTemplate(@"Subject",
                    new MessageTemplateToken[]
                    {
                        new PropertyToken("Message", "A multiline" + Environment.NewLine
                                                                   + "Message")
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
        Assert.Equal(@"[Error] A multiline" + Environment.NewLine
                                            + "Message" + Environment.NewLine
                                            + "System.ArgumentOutOfRangeException: Message of the exception"
                                            + " (Parameter 'parameter1')"
                                            + Environment.NewLine + "", actual.Subject);
        Assert.Equal("from@localhost.local", actual.From);
        Assert.Equal(new[] { "to@localhost.local" }, actual.To);
        Assert.True(actual.IsBodyHtml);
    }

    [Fact]
    public void WorksWithIBatchTextFormatter()
    {
        var emailConnectionInfo = new EmailConnectionInfo
        {
            ToEmail = "to@example.com",
            FromEmail = "from@localhost.local",
            IsBodyHtml = true
        };

        var emailTransport = new TestEmailTransport();
        var sink = new EmailSink(emailConnectionInfo, new HtmlTableFormatter(), new MessageTemplateTextFormatter(""),
            emailTransport);

        using (var emailLogger = new LoggerConfiguration()
                   .WriteTo.Sink(new PeriodicBatchingSink(sink, new PeriodicBatchingSinkOptions()))
                   .CreateLogger())
        {
            emailLogger.Information("Information");
            emailLogger.Warning("Warning");
            emailLogger.Error("<Error>");
        }

        var body = emailTransport.Sent.Single().Body;
        Assert.Equal("<table><tr>Information</tr><tr>Warning</tr><tr>&lt;Error&gt;</tr></table>", body);
    }

    static EmailSink CreateDefaultEmailSink(EmailConnectionInfo emailConnectionInfo, IEmailTransport transport)
    {
        var formatter = new MessageTemplateTextFormatter("[{Level}] {Message}{NewLine}{Exception}");
        var subjectLineFormatter = new MessageTemplateTextFormatter("[{Level}] {Message}{NewLine}{Exception}");

        var emailSink = new EmailSink(
            emailConnectionInfo,
            formatter,
            subjectLineFormatter,
            transport);
        return emailSink;
    }
}
