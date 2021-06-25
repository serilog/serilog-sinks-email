using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Debugging;
using Serilog.Events;
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

        [Fact(Skip = "Requires a smtp mail server")]
        public void WorksMulitpleEventsInOneMail()
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

        [Fact(Skip = "Requires a smtp mail server")]
        public void WorksWithIBatchTextFormatter()
        {
            var selfLogMessages = new List<string>();
            SelfLog.Enable(selfLogMessages.Add);

            var emailConnectionInfo = new EmailConnectionInfo
            {
                EmailSubject = "test subject",
                FromEmail = "from@smtpserver.local",
                ToEmail = "to@smtpserver.local",
                MailServer = "smtpserver.local",
            };
            using (var emailLogger = new LoggerConfiguration()
                .WriteTo.Email(emailConnectionInfo, new BatchFormatter())
                .CreateLogger())
            {
                emailLogger.Information("log1");
                emailLogger.Information("log2");
                emailLogger.Information("log3");
            }

            Assert.Equal(Enumerable.Empty<string>(), selfLogMessages);
        }

        private class BatchFormatter : IBatchTextFormatter
        {
            public void Format(LogEvent logEvent, TextWriter output)
            {
                output.Write("<tr>");
                logEvent.RenderMessage(output);
                output.WriteLine("</tr>");
            }

            public void WriteHeader(TextWriter output) => output.WriteLine("<table>");

            public void WriteFooter(TextWriter output) => output.WriteLine("</table>");
        }
    }
}
