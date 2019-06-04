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
using System.ComponentModel;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Serilog.Debugging;

namespace Serilog.Sinks.Email
{
    internal class SystemMailEmailTransport : IEmailTransport
    {
        private readonly SmtpClient _smtpClient;

        public SystemMailEmailTransport(EmailConnectionInfo connectionInfo)
        {
            _smtpClient = CreateSmtpClient(connectionInfo);
            _smtpClient.SendCompleted += SendCompletedCallback;
        }

        public async Task SendMailAsync(Email email)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(email.From),
                Subject = email.Subject,
                Body = email.Body,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = email.IsBodyHtml
            };

            foreach (var recipient in email.To)
            {
                mailMessage.To.Add(recipient);
            }

            await _smtpClient.SendMailAsync(mailMessage);
        }

        public void Dispose()
        {
            _smtpClient?.Dispose();
        }

        private SmtpClient CreateSmtpClient(EmailConnectionInfo connectionInfo)
        {
            var smtpClient = new SmtpClient();
            if (!string.IsNullOrWhiteSpace(connectionInfo.MailServer))
            {
                if (connectionInfo.NetworkCredentials == null)
                {
                    smtpClient.UseDefaultCredentials = true;
                }
                else
                {
                    smtpClient.Credentials = connectionInfo.NetworkCredentials;
                }

                smtpClient.Host = connectionInfo.MailServer;
                smtpClient.Port = connectionInfo.Port;
                smtpClient.EnableSsl = connectionInfo.EnableSsl;
            }

            return smtpClient;
        }

        /// <summary>
        ///     Reports if there is an error in sending an email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
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
    }
}
#endif