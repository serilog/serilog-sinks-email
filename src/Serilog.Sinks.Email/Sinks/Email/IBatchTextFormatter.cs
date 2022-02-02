using System;
using System.IO;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.Email
{
    /// <summary>
    /// Writes a header and a footer when handling batches of log events.
    /// <para/>
    /// Use this interface if you need to write a header and/or a footer before/after formatting a batch of log events.
    /// This is useful if you want to format log events inside a table of an html email.
    /// Use <see cref="WriteHeader"/> to write the opening tags (e.g. <c>&lt;html&gt;&lt;body&gt;&lt;table&gt;</c>)
    /// and <see cref="WriteFooter"/> to write the closing tags (e.g. <c>&lt;/table&gt;&lt;/body&gt;&lt;/html&gt;</c>).
    /// <para/>
    /// Make the text formatter passed to <see cref="Serilog.LoggerConfigurationEmailExtensions.Email(LoggerSinkConfiguration,EmailConnectionInfo,ITextFormatter,LogEventLevel,int,TimeSpan?,string)"/>
    /// also implement the <see cref="IBatchTextFormatter"/> interface in order to write a header and a footer to the same output as the <see cref="ITextFormatter"/>.
    /// </summary>
    public interface IBatchTextFormatter
    {
        /// <summary>
        /// Use this method to write a header just before a batch of log events is written to the <paramref name="output"/>.
        /// <para/>
        /// For example, write the opening tags (e.g. <c>&lt;html&gt;&lt;body&gt;&lt;table&gt;</c>) for an html email.
        /// </summary>
        /// <param name="output">The output where to write the header.</param>
        void WriteHeader(TextWriter output);

        /// <summary>
        /// Use this method to write a footer just after a batch of log events is written to the <paramref name="output"/>.
        /// <para/>
        /// For example, write the closing tags (e.g. <c>&lt;/table&gt;&lt;/body&gt;&lt;/html&gt;</c>) for an html email.
        /// </summary>
        /// <param name="output">The output where to write the footer.</param>
        void WriteFooter(TextWriter output);
    }
}
