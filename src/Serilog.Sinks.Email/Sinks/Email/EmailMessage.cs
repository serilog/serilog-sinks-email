using System.Collections.Generic;

namespace Serilog.Sinks.Email
{
    public class EmailMessage
    {
        public EmailMessage(string from, IEnumerable<string> to, string subject, string body, bool isBodyHtml)
        {
            From = from;
            To = to;
            Subject = subject;
            Body = body;
            IsBodyHtml = isBodyHtml;
        }
        public string From { get; }

        public string Subject { get; }

        public string Body { get; }

        public bool IsBodyHtml { get; }

        public IEnumerable<string> To { get; }
    }
}
