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
using System.IO;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using System.Linq;

namespace Serilog.Sinks.Email;

class EmailSink : IBatchedLogEventSink, IDisposable
{
    static readonly char[] MailAddressesSplitCharacters = { ';', ',' };

    readonly EmailConnectionInfo _connectionInfo;
    readonly IEmailTransport _emailTransport;

    readonly ITextFormatter _textFormatter;

    readonly ITextFormatter _subjectLineFormatter;

    /// <summary>
    /// Construct a sink emailing with the specified details.
    /// </summary>
    /// <param name="connectionInfo">Connection information used to construct the SMTP client and mail messages.</param>
    /// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
    /// <param name="subjectLineFormatter">Supplies culture-specific formatting information, or null.</param>
    /// <param name="emailTransport">The email transport to use.</param>
    /// <exception cref="System.ArgumentNullException">connectionInfo</exception>
    public EmailSink(EmailConnectionInfo connectionInfo, ITextFormatter textFormatter, ITextFormatter subjectLineFormatter, IEmailTransport emailTransport)
    {
        _connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof(connectionInfo));
        _emailTransport = emailTransport ?? throw new ArgumentNullException(nameof(emailTransport));
        _textFormatter = textFormatter;
        _subjectLineFormatter = subjectLineFormatter;
    }

    /// <summary>
    /// Emit a batch of log events, running asynchronously.
    /// </summary>
    /// <param name="events">The events to emit.</param>
    /// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
    /// not both.</remarks>
    public Task EmitBatchAsync(IEnumerable<LogEvent> events)
    {
        // ReSharper disable PossibleMultipleEnumeration

        if (events == null)
            throw new ArgumentNullException(nameof(events));

        var payload = new StringWriter();

        if (_textFormatter is IBatchTextFormatter batchTextFormatter)
        {
            batchTextFormatter.FormatBatch(events, payload);
        }
        else
        {
            foreach (var logEvent in events)
            {
                _textFormatter.Format(logEvent, payload);
            }
        }

        var subject = new StringWriter();
        _subjectLineFormatter.Format(events.OrderByDescending(e => e.Level).First(), subject);

        var email = new EmailMessage(
            _connectionInfo.FromEmail!,
            _connectionInfo.ToEmail!.Split(MailAddressesSplitCharacters, StringSplitOptions.RemoveEmptyEntries),
            subject.ToString(),
            payload.ToString(),
            _connectionInfo.IsBodyHtml);

        return _emailTransport.SendMailAsync(email);

        // ReSharper restore PossibleMultipleEnumeration
    }

    public Task OnEmptyBatchAsync()
    {
        return Task.FromResult(false);
    }

    public void Dispose()
    {
        _emailTransport.Dispose();
    }
}
