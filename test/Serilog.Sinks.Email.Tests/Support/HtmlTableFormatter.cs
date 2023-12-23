using System.Collections.Generic;
using System.IO;
using System.Net;
using Serilog.Events;

namespace Serilog.Sinks.Email.Tests;

class HtmlTableFormatter : IBatchTextFormatter
{
    public void FormatBatch(IEnumerable<LogEvent> logEvents, TextWriter output)
    {
        output.Write("<table>");
        foreach (var logEvent in logEvents)
        {
            Format(logEvent, output);
        }

        output.Write("</table>");
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        output.Write("<tr>");
        using var buffer = new StringWriter();
        logEvent.RenderMessage(buffer);
        output.Write(WebUtility.HtmlEncode(buffer.ToString()));
        output.Write("</tr>");
    }
}
