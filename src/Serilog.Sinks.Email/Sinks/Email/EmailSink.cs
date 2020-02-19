// Copyright 2014 Serilog Contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#if SYSTEM_NET

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using Serilog.Formatting.Display;
using System.Linq;

namespace Serilog.Sinks.Email
{
    class EmailSink : IBatchedLogEventSink, ILogEventSink, IDisposable
    {
        readonly EmailConnectionInfo _connectionInfo;

        readonly SmtpClient _smtpClient;

        readonly ITextFormatter _textFormatter;

        readonly ITextFormatter _subjectLineFormatter;

        private static Task CompletedTask = Task.FromResult(false); // The value is irrelevant as it just marks a completed operation.

        /// <summary>
        /// Construct a sink emailing with the specified details.
        /// </summary>
        /// <param name="connectionInfo">Connection information used to construct the SMTP client and mail messages.</param>
        /// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
        /// <param name="subjectLineFormatter">Supplies culture-specific formatting information, or null.</param>
        public EmailSink(EmailConnectionInfo connectionInfo, ITextFormatter textFormatter, ITextFormatter subjectLineFormatter)
        {
            if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));

            _connectionInfo = connectionInfo;
            _textFormatter = textFormatter;
            _subjectLineFormatter = subjectLineFormatter;
            _smtpClient = CreateSmtpClient();
            _smtpClient.SendCompleted += SendCompletedCallback;
        }

        /// <summary>
        /// Reports if there is an error in sending an email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void SendCompletedCallback(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                SelfLog.WriteLine("Received failed result {0}: {1}", "Cancelled", e.Error);
            }
            if (e.Error != null)
            {
                SelfLog.WriteLine("Received failed result {0}: {1}", "Error", e.Error);
            }
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">If true, called because the object is being disposed; if false,
        /// the object is being disposed from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _smtpClient.Dispose();
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            SelfLog.WriteLine("The email sink only supports batched log events.");
        }

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        public async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var payload = new StringWriter();

            foreach (var logEvent in events)
            {
                _textFormatter.Format(logEvent, payload);
            }

            var subject = new StringWriter();
            _subjectLineFormatter.Format(events.OrderByDescending(e => e.Level).First(), subject);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_connectionInfo.FromEmail),
                Subject = subject.ToString(),
                Body = payload.ToString(),
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = _connectionInfo.IsBodyHtml
            };

            foreach (var recipient in _connectionInfo.ToEmail.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.To.Add(recipient);
            }

            await _smtpClient.SendMailAsync(mailMessage);
        }

        /// <summary>
        /// Allows sinks to perform periodic work without requiring additional threads
        /// or timers (thus avoiding additional flush/shut-down complexity).
        /// </summary>
        /// <returns></returns>
        public Task OnEmptyBatchAsync()
        {
            return CompletedTask;
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrWhiteSpace(_connectionInfo.MailServer))
            {
                if (_connectionInfo.NetworkCredentials == null)
                    smtpClient.UseDefaultCredentials = true;
                else
                    smtpClient.Credentials = _connectionInfo.NetworkCredentials;

                smtpClient.Host = _connectionInfo.MailServer;
                smtpClient.Port = _connectionInfo.Port;
                smtpClient.EnableSsl = _connectionInfo.EnableSsl;
            }

            return smtpClient;
        }
    }
}
#endif
