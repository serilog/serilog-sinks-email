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
using System.Linq;
using Serilog.Core;

namespace Serilog.Sinks.Email;

class EmailSink : IBatchedLogEventSink, IDisposable
{
    readonly EmailSinkOptions _sinkOptions;
    readonly IEmailTransport _emailTransport;

    /// <summary>
    /// Construct a sink emailing with the specified details.
    /// </summary>
    /// <param name="options">Connection information used to construct the SMTP client and mail messages.</param>
    /// <param name="emailTransport">The email transport to use.</param>
    /// <exception cref="System.ArgumentNullException">connectionInfo</exception>
    public EmailSink(EmailSinkOptions options, IEmailTransport emailTransport)
    {
        _sinkOptions = options ?? throw new ArgumentNullException(nameof(options));
        _emailTransport = emailTransport ?? throw new ArgumentNullException(nameof(emailTransport));
    }

    /// <summary>
    /// Emit a batch of log events, running asynchronously.
    /// </summary>
    /// <param name="events">The events to emit.</param>
    public Task EmitBatchAsync(IReadOnlyCollection<LogEvent> events)
    {
        if (events == null)
            throw new ArgumentNullException(nameof(events));

        var body = new StringWriter();

        if (_sinkOptions.Body is IBatchTextFormatter batchTextFormatter)
        {
            batchTextFormatter.FormatBatch(events, body);
        }
        else
        {
            foreach (var logEvent in events)
            {
                _sinkOptions.Body.Format(logEvent, body);
            }
        }

        var subject = ComputeMailSubject(_sinkOptions.Subject, events);

        var email = new EmailMessage(
            _sinkOptions.From,
            _sinkOptions.To,
            subject,
            body.ToString(),
            _sinkOptions.IsBodyHtml);

        return _emailTransport.SendMailAsync(email);
    }

    internal static string ComputeMailSubject(ITextFormatter subjectLineFormatter, IEnumerable<LogEvent> events)
    {
        var subject = new StringWriter();
        subjectLineFormatter.Format(events.OrderByDescending(e => e.Level).First(), subject);
        var subjectAsText = subject.ToString();
        var firstLineOfSubject = subjectAsText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                     .FirstOrDefault() ?? string.Empty;
        return firstLineOfSubject;
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
