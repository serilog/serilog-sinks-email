using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using Xunit;

namespace Serilog.Sinks.Email.Tests
{
    public class EmailSinkTests
    {
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

        [Fact]
        public void MultilineMessageCreatesSubjectWithTheFirstLineOnly()
        {
            var subjectLineFormatter = new MessageTemplateTextFormatter("{Message}", null);

            var logEvents = new[]
            {
                new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, new Exception("An exception occured"),
                    new MessageTemplate(@"Subject",
                        new MessageTemplateToken[]{new PropertyToken("Message", "A multiline" + Environment.NewLine + "Message")})
                    //        Enumerable.Empty<MessageTemplateToken>())
                    , Enumerable.Empty<LogEventProperty>())
            };
            var mailSubject = EmailSink.ComputeMailSubject(subjectLineFormatter, logEvents);

            Assert.Equal("A multiline", mailSubject);
        }
    }
}
