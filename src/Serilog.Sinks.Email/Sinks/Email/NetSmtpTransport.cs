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
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Serilog.Sinks.Email;

class NetSmtpTransport(EmailSinkOptions options) : IEmailTransport
{
    public async Task SendMail(EmailMessage emailMessage)
    {
        using (SmtpClient client = new SmtpClient(options.Host))
        {
            client.Port = options.Port; // or 25 or any other port your server uses
            client.EnableSsl = false; // Set to true if your server requires SSL            
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            if (options.UseDefaultCredentials)
            {
                client.UseDefaultCredentials = true;
            }
            else {
                // Use network credentials with NTLM authentication
                client.UseDefaultCredentials = false;
                client.Credentials = options.Credentials;
            }

            // Compose the email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(emailMessage.From);

            foreach(var recipient in emailMessage.To) { 
                mail.To.Add(recipient);
            }
            mail.Subject = emailMessage.Subject;
            mail.Body = emailMessage.Body;
            mail.IsBodyHtml = options.IsBodyHtml;

            // Send the email            
            client.Send(mail);
            Console.WriteLine("Email sent successfully!");
        }
    }

    public void Dispose()
    {
    }
}
