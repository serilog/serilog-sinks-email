using System;
using System.Collections.Generic;
using System.IO;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Email
{
    /// <summary>
    /// An extension of <see cref="ITextFormatter"/> for handling batches of log events.
    /// Use this interface when more control over the formatting of multiple log events is required.
    /// <para/>
    /// Pass an <see cref="IBatchTextFormatter"/> instance for the <see cref="ITextFormatter"/> argument when configuring the email sink with
    /// <see cref="Serilog.LoggerConfigurationEmailExtensions.Email(LoggerSinkConfiguration,EmailConnectionInfo,ITextFormatter,LogEventLevel,int,TimeSpan?,string)"/>.
    /// <example>
    /// This interface might be used to write a header and/or a footer before/after formatting multiple log events,
    /// for example to format the events inside a table of an html email. It could also be used to group events by log level.
    /// <para>
    /// <code>
    /// class HtmlTableFormatter : IBatchTextFormatter
    /// {
    ///     public void FormatBatch(IEnumerable&lt;LogEvent&gt; logEvents, TextWriter output)
    ///     {
    ///         output.Write("&lt;table&gt;");
    ///         foreach (var logEvent in logEvents)
    ///         {
    ///             Format(logEvent, output);
    ///         }
    ///         output.Write("&lt;/table&gt;");
    ///     }
    ///
    ///     public void Format(LogEvent logEvent, TextWriter output)
    ///     {
    ///         output.Write("&lt;tr&gt;");
    ///         using var buffer = new StringWriter();
    ///         logEvent.RenderMessage(buffer);
    ///         output.Write(WebUtility.HtmlEncode(buffer.ToString()));
    ///         output.Write("&lt;/tr&gt;");
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </example>
    /// </summary>
    public interface IBatchTextFormatter : ITextFormatter
    {
        /// <summary>Format the log events into the output.</summary>
        /// <param name="logEvents">The events to format.</param>
        /// <param name="output">The output.</param>
        void FormatBatch(IEnumerable<LogEvent> logEvents, TextWriter output);
    }
}
