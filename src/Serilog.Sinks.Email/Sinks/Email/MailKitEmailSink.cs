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

#if MAIL_KIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace Serilog.Sinks.Email
{
    class EmailSink : PeriodicBatchingSink
    {
        readonly EmailConnectionInfo _connectionInfo;

        readonly MimeKit.InternetAddress _fromAddress;
        readonly IEnumerable<MimeKit.InternetAddress> _toAddresses;

        readonly ITextFormatter _textFormatter;

        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 100;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Construct a sink emailing with the specified details.
        /// </summary>
        /// <param name="connectionInfo">Connection information used to construct the SMTP client and mail messages.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
        public EmailSink(EmailConnectionInfo connectionInfo, int batchSizeLimit, TimeSpan period, ITextFormatter textFormatter)
            : base(batchSizeLimit, period)
        {
            if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));

            _connectionInfo = connectionInfo;
            _fromAddress = MimeKit.MailboxAddress.Parse(_connectionInfo.FromEmail);
            _toAddresses = connectionInfo
                .ToEmail
                .Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(MimeKit.MailboxAddress.Parse)
                .ToArray();

            _textFormatter = textFormatter;
        }
        
        private MimeKit.MimeMessage CreateMailMessage(string payload)
        {
            var mailMessage = new MimeKit.MimeMessage();
            mailMessage.From.Add(_fromAddress);
            mailMessage.To.AddRange(_toAddresses);
            mailMessage.Subject = _connectionInfo.EmailSubject;
            mailMessage.Body = _connectionInfo.IsBodyHtml
                ? new MimeKit.BodyBuilder { HtmlBody = payload }.ToMessageBody()
                : new MimeKit.BodyBuilder { TextBody = payload }.ToMessageBody();
            return mailMessage;            
        }

        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
        /// not both.</remarks>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var payload = new StringWriter();

            foreach (var logEvent in events)
            {
                _textFormatter.Format(logEvent, payload);
            }

            var mailMessage = CreateMailMessage(payload.ToString());

            try
            {
                using (var smtpClient = OpenConnectedSmtpClient())
                {
                    await smtpClient.SendAsync(mailMessage);
                    await smtpClient.DisconnectAsync(quit: true);
                }
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Failed to send email: {0}", ex.ToString());
            }
        }

        private SmtpClient OpenConnectedSmtpClient()
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrWhiteSpace(_connectionInfo.MailServer))
            {
                smtpClient.Connect(
                    _connectionInfo.MailServer, _connectionInfo.Port,
                    useSsl: _connectionInfo.EnableSsl);

                if (_connectionInfo.NetworkCredentials != null)
                {
                    smtpClient.Authenticate(
                        Encoding.UTF8,
                        _connectionInfo.NetworkCredentials.GetCredential(
                            _connectionInfo.MailServer, _connectionInfo.Port, "smtp"));
                }
            }
            return smtpClient;
        }
    }
}
#endif