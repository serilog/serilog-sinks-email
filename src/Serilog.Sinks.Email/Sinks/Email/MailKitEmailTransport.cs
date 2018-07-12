﻿// Copyright 2014 Serilog Contributors
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

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Serilog.Sinks.Email
{
    class MailKitEmailTransport: IEmailTransport
    {
        private readonly EmailConnectionInfo _connectionInfo;

        public MailKitEmailTransport(EmailConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public async Task SendMailAsync(Email email)
        {
            var fromAddress = MailboxAddress.Parse(email.From);
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(fromAddress);
            mimeMessage.To.AddRange(email.To.Select(MailboxAddress.Parse));
            mimeMessage.Subject = email.Subject;
            mimeMessage.Body = _connectionInfo.IsBodyHtml
                ? new BodyBuilder { HtmlBody = email.Body }.ToMessageBody()
                : new BodyBuilder { TextBody = email.Body }.ToMessageBody();
            using (var smtpClient = OpenConnectedSmtpClient())
            {
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(quit: true);
            }
        }
        private SmtpClient OpenConnectedSmtpClient()
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrWhiteSpace(_connectionInfo.MailServer))
            {
                if (_connectionInfo.ServerCertificateValidationCallback != null)
                {
                    smtpClient.ServerCertificateValidationCallback += _connectionInfo.ServerCertificateValidationCallback;
                }

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

        public void Dispose()
        {
        }
    }
}
#endif