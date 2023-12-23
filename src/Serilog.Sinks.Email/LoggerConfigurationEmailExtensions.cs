// Copyright © Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Net;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Email;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
// ReSharper disable MemberCanBePrivate.Global

namespace Serilog;

/// <summary>
/// Adds the WriteTo.Email() extension method to <see cref="LoggerConfiguration"/>.
/// </summary>
public static class LoggerConfigurationEmailExtensions
{
    const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
    const int DefaultBatchPostingLimit = 100;
    static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);
    const string DefaultSubject = "Log Messages";
    const int DefaultQueueLimit = 10000;

    /// <summary>
    /// Adds a sink that sends log events via email.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="fromEmail">The email address emails will be sent from.</param>
    /// <param name="toEmail">The email address emails will be sent to. Multiple addresses can be separated
    /// with a comma or semicolon.</param>
    /// <param name="mailServer">The SMTP email server to use</param>
    /// <param name="networkCredential">The network credentials to use to authenticate with mailServer</param>
    /// <param name="outputTemplate">A message template describing the format used to write to the sink.
    /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
    /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
    /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
    /// <param name="period">The time to wait between checking for event batches.</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
    /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
    /// <returns>
    /// Logger configuration, allowing configuration to continue.
    /// </returns>
    /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
    public static LoggerConfiguration Email(
        this LoggerSinkConfiguration loggerConfiguration,
        string fromEmail,
        string toEmail,
        string mailServer,
        ICredentialsByHost? networkCredential = null,
        string? outputTemplate = null,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        int? batchPostingLimit = null,
        TimeSpan? period = null,
        IFormatProvider? formatProvider = null,
        string? mailSubject = null)
    {
        if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
        if (fromEmail == null) throw new ArgumentNullException(nameof(fromEmail));
        if (toEmail == null) throw new ArgumentNullException(nameof(toEmail));
        if (mailServer == null) throw new ArgumentNullException(nameof(mailServer));

        var connectionInfo = new EmailConnectionInfo
        {
            FromEmail = fromEmail,
            ToEmail = toEmail,
            MailServer = mailServer,
            NetworkCredentials = networkCredential
        };

        var batchingOptions = new PeriodicBatchingSinkOptions
        {
            BatchSizeLimit = batchPostingLimit ?? DefaultBatchPostingLimit,
            Period = period ?? DefaultPeriod,
            EagerlyEmitFirstEvent = false,  // set default to false, not usable for emailing
            QueueLimit = DefaultQueueLimit,
        };

        return Email(
            loggerConfiguration,
            connectionInfo,
            new MessageTemplateTextFormatter(outputTemplate ?? DefaultOutputTemplate, formatProvider),
            new MessageTemplateTextFormatter(mailSubject ?? DefaultSubject, formatProvider),
            restrictedToMinimumLevel,
            batchingOptions);
    }

    /// <summary>
    /// Adds a sink that sends log events via email.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="connectionInfo">The connection info used for</param>
    /// <param name="bodyFormatter">The <see cref="ITextFormatter"/> or <see cref="IBatchTextFormatter"/> implementation
    /// to write log entries to email. Specify <c>null</c> to use the default body. . Consider using
    /// <see cref="MessageTemplateTextFormatter"/> or <c>Serilog.Expressions</c> templates.</param>
    /// <param name="subjectFormatter">The <see cref="ITextFormatter"/> implementation to format email subjects. Specify
    /// <c>null</c> to use the default subject. Consider using <see cref="MessageTemplateTextFormatter"/> or
    /// <c>Serilog.Expressions</c> templates.</param>
    /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
    /// <param name="batchingOptions">Optionally, a <see cref="PeriodicBatchingSinkOptions"/> to control background batching.</param>
    /// <param name="defaultFormatProvider">An <see cref="IFormatProvider"/> to use when constructing the default subject or
    /// body formatters; ignored when subject and body formatters are supplied.</param>
    /// <returns>
    /// Logger configuration, allowing configuration to continue.
    /// </returns>
    /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
    public static LoggerConfiguration Email(
        this LoggerSinkConfiguration loggerConfiguration,
        EmailConnectionInfo connectionInfo,
        ITextFormatter? bodyFormatter = null,
        ITextFormatter? subjectFormatter = null,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        PeriodicBatchingSinkOptions? batchingOptions = null,
        IFormatProvider? defaultFormatProvider = null)
    {
        if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));

        bodyFormatter ??= new MessageTemplateTextFormatter(DefaultOutputTemplate, defaultFormatProvider);
        subjectFormatter ??= new MessageTemplateTextFormatter(DefaultSubject, defaultFormatProvider);

        batchingOptions ??= new PeriodicBatchingSinkOptions
        {
            BatchSizeLimit = DefaultBatchPostingLimit,
            Period = DefaultPeriod,
            EagerlyEmitFirstEvent = false,  // set default to false, not usable for emailing
            QueueLimit = DefaultQueueLimit
        };

        var transport = new MailKitEmailTransport(connectionInfo);
        var sink = new EmailSink(connectionInfo, bodyFormatter, subjectFormatter, transport);

        var batchingSink = new PeriodicBatchingSink(sink, batchingOptions);

        return loggerConfiguration.Sink(batchingSink, restrictedToMinimumLevel);
    }
}
