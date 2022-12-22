using Moq;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Debugging;
using Xunit;

namespace Serilog.Sinks.Email.Tests
{
    public class SqlServerEmailSinkTests
    {
        [Fact]
        [UseCulture("en-us")]
        public async Task SendSqlServerEmailIsCorrectlyCalledWhenEventAreLogged()
        {
            EmailMessage actual = null;
            var (emailConnectionInfoMock, emailTransportMock) =
                CreateSqlServerEmailTransportConnectionInfoMocks(email => actual = email);
            var emailConnectionInfo = emailConnectionInfoMock.Object;
            emailConnectionInfo.ToEmail = "to@localhost.local";
            emailConnectionInfo.FromEmail = "from@localhost.local";
            emailConnectionInfo.IsBodyHtml = true;
            var emailSink = CreateDefaultEmailSink(emailConnectionInfoMock.Object);

            await emailSink.EmitBatchAsync(new[] {
                new LogEvent(
                    DateTimeOffset.Now,
                    LogEventLevel.Error,
                    new ArgumentOutOfRangeException("parameter1", "Message of the exception"),
                    new MessageTemplate(@"Subject",
                        new MessageTemplateToken[]
                        {
                            new PropertyToken("Message", "A multiline" + Environment.NewLine
                                                                       + "Message")
                        })
                    , Enumerable.Empty<LogEventProperty>())});
            emailSink.Dispose();

            emailTransportMock.Verify(et => et.SendMailAsync(It.IsAny<EmailMessage>()), Times.Once);

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
        public void WorksWithSqlServerConnectionInfo()
        {
            var body = "SendMailAsync was not called";
            var selfLogMessages = new List<string>();
            SelfLog.Enable(selfLogMessages.Add);

            var emailConnectionInfo = new Mock<SqlServerEmailConnectionInfo>
            {
                Object =
                {
                    ToEmail = "to@example.com",
                    MailProfileName = "My_SMTP_Profile",
                    SqlConnectionString = "Data Source=(local);Integrated Security=True"
                }
            };
            var emailTransport = new Mock<IEmailTransport>();
            emailTransport.Setup(x => x.SendMailAsync(It.IsAny<EmailMessage>())).Callback<EmailMessage>(m => body = m.Body).Returns(Task.FromResult(false));
            emailConnectionInfo.Setup(x => x.CreateEmailTransport()).Returns(emailTransport.Object);

            using (var emailLogger = new LoggerConfiguration()
                       .WriteTo.Email(emailConnectionInfo.Object, "[{Level}] {Message}{NewLine}{Exception}")
                       .CreateLogger())
            {
                emailLogger.Information("Information");
            }

            Assert.Empty(selfLogMessages);
            Assert.Equal("[Information] Information\r\n", body);
        }

        private EmailSink CreateDefaultEmailSink(SqlServerEmailConnectionInfo sqlServerEmailConnectionInfo)
        {
            var formatter = new MessageTemplateTextFormatter("[{Level}] {Message}{NewLine}{Exception}", null);
            var subjectLineFormatter = new MessageTemplateTextFormatter("[{Level}] {Message}{NewLine}{Exception}", null);

            var emailSink = new EmailSink(
                sqlServerEmailConnectionInfo,
                formatter,
                subjectLineFormatter);
            return emailSink;
        }

        private (Mock<SqlServerEmailConnectionInfo> EmailConnectionInfoMock, Mock<IEmailTransport> EmailTransportMock)
            CreateSqlServerEmailTransportConnectionInfoMocks(Action<EmailMessage> emailSend)
        {
            var emailTransportMock = new Mock<IEmailTransport>();
            emailTransportMock.Setup(et => et.SendMailAsync(It.IsAny<EmailMessage>()))
                .Callback(emailSend)
                .Returns(Task.Factory.StartNew(() => { }));
            var emailTransport = emailTransportMock.Object;
            var emailConnectionInfoMock = new Mock<SqlServerEmailConnectionInfo>();
            emailConnectionInfoMock
                .Setup(eci => eci.CreateEmailTransport())
                .Returns(emailTransport);
            return (emailConnectionInfoMock, emailTransportMock);
        }
    }
}
