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
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Email;
// ReSharper disable MemberCanBePrivate.Global

namespace Serilog;

/// <summary>
/// Adds the WriteTo.Email() extension method to <see cref="LoggerConfiguration"/>.
/// </summary>
public static class LoggerConfigurationEmailExtensions
{
    static readonly TimeSpan DefaultBufferingTimeLimit = TimeSpan.FromSeconds(30);
    const int DefaultQueueLimit = 10000;

    /// <summary>
    /// Adds a sink that sends log events via email.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="from">The email address emails will be sent from.</param>
    /// <param name="to">The email address emails will be sent to. Multiple addresses can be separated
    /// with commas or semicolons.</param>
    /// <param name="host">The SMTP email server to use</param>
    /// <param name="credentials">The network credentials to use to authenticate with mailServer</param>
    /// <param name="body">A message template describing the format used to write to the sink.
    /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
    /// <param name="subject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
    /// <param name="port">Gets or sets the port used for the SMTP connection. The default is 25.</param>
    /// <param name="enableSSL">Enables SSL. Default is False.</param>
    /// <param name="isBodyHtml">Enable Body as HTML. Default is False.</param>
    /// <param name="useDefaultCredentials">Use the default credentials. Default is False.</param>
    /// <param name="restrictedToMinimumLevel">The minimum level for
    /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
    /// <param name="levelSwitch">A switch allowing the pass-through minimum level
    /// to be changed at runtime.</param>
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
        ICredentialsByHost? credentials = null,
        string? subject = null,
        string? body = null,
        bool enableSSL = false,
        bool isBodyHtml = false,
        IFormatProvider? formatProvider = null,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        LoggingLevelSwitch? levelSwitch = null,
        bool useDefaultCredentials = false)
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
            EnableSSL = enableSSL,
            Credentials = credentials,
            IsBodyHtml = isBodyHtml,
            UseDefaultCredentials = useDefaultCredentials
        };

        if (subject != null)
            connectionInfo.Subject = new MessageTemplateTextFormatter(subject, formatProvider);

        if (body != null)
            connectionInfo.Body = new MessageTemplateTextFormatter(body, formatProvider);

        return Email(
            loggerConfiguration,
            connectionInfo,
            null,
            restrictedToMinimumLevel,
            levelSwitch);
    }

    /// <summary>
    /// Adds a sink that sends log events via email.
    /// </summary>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="options">The connection info used for</param>
    /// <param name="batchingOptions">Optionally, a <see cref="BatchingOptions"/> to control background batching.</param>
    /// <param name="restrictedToMinimumLevel">The minimum level for
    /// events passed through the sink. Ignored when <paramref name="levelSwitch"/> is specified.</param>
    /// <param name="levelSwitch">A switch allowing the pass-through minimum level
    /// to be changed at runtime.</param>
    /// <returns>
    /// Logger configuration, allowing configuration to continue.
    /// </returns>
    /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
    public static LoggerConfiguration Email(
        this LoggerSinkConfiguration loggerConfiguration,
        EmailSinkOptions options,
        BatchingOptions? batchingOptions = null,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
        LoggingLevelSwitch? levelSwitch = null)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        batchingOptions ??= new BatchingOptions
        {
            // Batching not used by default: fire off an email immediately upon receiving each event.
            BatchSizeLimit = 1,
            BufferingTimeLimit = DefaultBufferingTimeLimit,
            EagerlyEmitFirstEvent = true,
            QueueLimit = DefaultQueueLimit,
        };

        var transport = new NetSmtpTransport(options);
        var sink = new EmailSink(options, transport);

        return loggerConfiguration.Sink(sink, batchingOptions, restrictedToMinimumLevel, levelSwitch);
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
