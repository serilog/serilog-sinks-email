using System.Collections.Generic;
using System.Linq;
using Serilog.Debugging;
using Xunit;

namespace Serilog.Sinks.Email.Tests
{
    public class EmailSinkTests
    {
        [Fact(Skip="Requires a localhost mail server")]
        public void Works()
        {
            var selfLogMessages = new List<string>();
            SelfLog.Enable(selfLogMessages.Add);

            var emailLogger = new LoggerConfiguration()
                .WriteTo.Email(
                    fromEmail: "from@localhost.local",
                    toEmail: "to@localhost.local",
                    mailServer: "localhost",
                    outputTemplate: "[{Level}] {Message}{NewLine}{Exception}",
                    mailSubject: "subject")
                .CreateLogger();

            emailLogger.Information("test {test}", "test");
            emailLogger.Dispose();

            Assert.Equal(Enumerable.Empty<string>(), selfLogMessages);
        }
    }
}
