using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serilog.Sinks.Email.Tests.Support;

class TestEmailTransport : IEmailTransport
{
    public List<EmailMessage> Sent { get; } = new();
    public bool IsDisposed { get; set; }

    public void Dispose()
    {
        IsDisposed = true;
    }

    public Task SendMail(EmailMessage emailMessage)
    {
        Sent.Add(emailMessage);
        return Task.CompletedTask;
    }
}
