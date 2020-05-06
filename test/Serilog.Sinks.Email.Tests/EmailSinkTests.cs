using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;
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
    }
}
