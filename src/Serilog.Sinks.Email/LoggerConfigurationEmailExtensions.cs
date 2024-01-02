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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using MailKit.Security;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Email;
using Serilog.Sinks.PeriodicBatching;
// ReSharper disable MemberCanBePrivate.Global

namespace Serilog;

/// <summary>
/// Adds the WriteTo.Email() extension method to <see cref="LoggerConfiguration"/>.
/// </summary>
public static class LoggerConfigurationEmailExtensions
{
    const int DefaultBatchPostingLimit = 100;
    static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);
    const int DefaultQueueLimit = 10000;

    /// <summary>
    /// Adds a sink that sends log events via email.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="from">The email address emails will be sent from.</param>
    /// <param name="to">The email address emails will be sent to. Multiple addresses can be separated
    /// with commas or semicolons.</param>
    /// <param name="host">The SMTP email server to use</param>
    /// <param name="connectionSecurity">Choose the security applied to the SMTP connection. This enumeration type
    /// is supplied by MailKit; see <see cref="SecureSocketOptions"/> for supported values. The default is
    /// <see cref="SecureSocketOptions.Auto"/>.</param>
    /// <param name="credentials">The network credentials to use to authenticate with mailServer</param>
    /// <param name="body">A message template describing the format used to write to the sink.
    /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
    /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
    /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
    /// <param name="bufferingTimeLimit">The time to wait between checking for event batches.</param>
    /// <param name="isBodyHtml"></param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
    /// <param name="subject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
    /// <param name="port">Gets or sets the port used for the SMTP connection. The default is 25.</param>
    /// <returns>
    /// Logger configuration, allowing configuration to continue.
    /// </returns>
    /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
    public static LoggerConfiguration Email(
        this LoggerSinkConfiguration loggerConfiguration,
        string from,
        string to,
        string host,
        int port = EmailSinkOptions.DefaultPort,
        SecureSocketOptions connectionSecurity = EmailSinkOptions.DefaultConnectionSecurity,
        ICredentialsByHost? credentials = null,
        string? subject = null,
        string? body = null,
        bool isBodyHtml = false,
        IFormatProvider? formatProvider = null,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        int batchSizeLimit = DefaultBatchPostingLimit,
        TimeSpan? bufferingTimeLimit = null)
    {
        if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (to == null) throw new ArgumentNullException(nameof(to));
        if (host == null) throw new ArgumentNullException(nameof(host));

        var connectionInfo = new EmailSinkOptions
        {
            From = from,
            To = SplitToAddresses(to),
            Host = host,
            Port = port,
            ConnectionSecurity = connectionSecurity,
            Credentials = credentials,
            IsBodyHtml = isBodyHtml
        };

        if (subject != null)
            connectionInfo.Subject = new MessageTemplateTextFormatter(subject, formatProvider);

        if (body != null)
            connectionInfo.Body = new MessageTemplateTextFormatter(body, formatProvider);

        var batchingOptions = new PeriodicBatchingSinkOptions
        {
            BatchSizeLimit = batchSizeLimit,
            Period = bufferingTimeLimit ?? DefaultPeriod,
            EagerlyEmitFirstEvent = false,
            QueueLimit = DefaultQueueLimit,
        };

        return Email(
            loggerConfiguration,
            connectionInfo,
            batchingOptions,
            restrictedToMinimumLevel);
    }

    /// <summary>
    /// Adds a sink that sends log events via email.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="options">The connection info used for</param>
    /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
    /// <param name="batchingOptions">Optionally, a <see cref="PeriodicBatchingSinkOptions"/> to control background batching.</param>
    /// <returns>
    /// Logger configuration, allowing configuration to continue.
    /// </returns>
    /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
    public static LoggerConfiguration Email(
        this LoggerSinkConfiguration loggerConfiguration,
        EmailSinkOptions options,
        PeriodicBatchingSinkOptions? batchingOptions = null,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        batchingOptions ??= new PeriodicBatchingSinkOptions
        {
            BatchSizeLimit = DefaultBatchPostingLimit,
            Period = DefaultPeriod,
            EagerlyEmitFirstEvent = false,
            QueueLimit = DefaultQueueLimit
        };

        var transport = new MailKitEmailTransport(options);
        var sink = new EmailSink(options, transport);

        var batchingSink = new PeriodicBatchingSink(sink, batchingOptions);

        return loggerConfiguration.Sink(batchingSink, restrictedToMinimumLevel);
    }


    internal static List<string> SplitToAddresses(string? toEmail)
    {
        return (toEmail ?? "")
            .Split(';', ',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();
    }
}
