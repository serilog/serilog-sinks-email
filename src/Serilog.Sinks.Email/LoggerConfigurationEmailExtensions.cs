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

using System;
using System.Collections.Generic;
using System.Net;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Email;
using Serilog.Formatting;
using System.Net.Security;

namespace Serilog
{
    /// <summary>
    /// Adds the WriteTo.Email() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationEmailExtensions
    {
        const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmail">The email address emails will be sent to</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <param name="serverCertificateValidationCallback">Option to add callback for certificate validation on client Connect</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = EmailSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            string mailSubject = EmailConnectionInfo.DefaultSubject,
            RemoteCertificateValidationCallback serverCertificateValidationCallback = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (fromEmail == null) throw new ArgumentNullException("fromEmail");
            if (toEmail == null) throw new ArgumentNullException("toEmail");

            var connectionInfo = new EmailConnectionInfo
            {
                FromEmail = fromEmail,
                ToEmail = toEmail,
                EmailSubject = mailSubject,
                ServerCertificateValidationCallback = serverCertificateValidationCallback
            };

            return Email(loggerConfiguration, connectionInfo, outputTemplate, restrictedToMinimumLevel, batchPostingLimit, period, formatProvider);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmail">The email address emails will be sent to</param>
        /// <param name="mailServer">The SMTP email server to use</param>
        /// <param name="networkCredential">The network credentials to use to authenticate with mailServer</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <param name="serverCertificateValidationCallback">Option to add callback for certificate validation on client Connect</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string mailServer,
            ICredentialsByHost networkCredential = null,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = EmailSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            string mailSubject = EmailConnectionInfo.DefaultSubject,
            RemoteCertificateValidationCallback serverCertificateValidationCallback = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (fromEmail == null) throw new ArgumentNullException("fromEmail");
            if (toEmail == null) throw new ArgumentNullException("toEmail");

            var connectionInfo = new EmailConnectionInfo
            {
                FromEmail = fromEmail,
                ToEmail = toEmail,
                MailServer = mailServer,
                NetworkCredentials = networkCredential,
                EmailSubject = mailSubject,
                ServerCertificateValidationCallback = serverCertificateValidationCallback
            };

            return Email(loggerConfiguration, connectionInfo, outputTemplate, restrictedToMinimumLevel, batchPostingLimit, period, formatProvider);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmails">The email addresses emails will be sent to</param>
        /// <param name="mailServer">The SMTP email server to use</param>
        /// <param name="networkCredential">The network credentials to use to authenticate with mailServer</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <param name="serverCertificateValidationCallback">Option to add callback for certificate validation on client Connect</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            IEnumerable<string> toEmails,
            string mailServer,
            ICredentialsByHost networkCredential = null,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = EmailSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            string mailSubject = EmailConnectionInfo.DefaultSubject,
            RemoteCertificateValidationCallback serverCertificateValidationCallback = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException("loggerConfiguration");
            if (fromEmail == null) throw new ArgumentNullException("fromEmail");
            if (toEmails == null) throw new ArgumentNullException("toEmails");

            var connectionInfo = new EmailConnectionInfo
            {
                FromEmail = fromEmail,
                ToEmail = string.Join(";", toEmails),
                MailServer = mailServer,
                NetworkCredentials = networkCredential,
                EmailSubject = mailSubject,
                ServerCertificateValidationCallback = serverCertificateValidationCallback
            };

            return Email(loggerConfiguration, connectionInfo, outputTemplate, restrictedToMinimumLevel, batchPostingLimit, period, formatProvider, mailSubject);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="connectionInfo">The connection info used for </param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            EmailConnectionInfo connectionInfo,
            string outputTemplate = DefaultOutputTemplate,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = EmailSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            string mailSubject = EmailConnectionInfo.DefaultSubject)
        {
            if (connectionInfo == null) throw new ArgumentNullException("connectionInfo");

            if (!string.IsNullOrEmpty(connectionInfo.EmailSubject))
            {
                mailSubject = connectionInfo.EmailSubject;
            }

            var defaultedPeriod = period ?? EmailSink.DefaultPeriod;
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var subjectLineFormatter = new MessageTemplateTextFormatter(mailSubject, formatProvider);

            return loggerConfiguration.Sink(
                new EmailSink(connectionInfo, batchPostingLimit, defaultedPeriod, formatter, subjectLineFormatter),
                restrictedToMinimumLevel);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="connectionInfo">The connection info used for </param>
        /// <param name="restrictedToMinimumLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchPostingLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="textFormatter">ITextFormatter implementation to write log entry to email.</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            EmailConnectionInfo connectionInfo,
            ITextFormatter textFormatter,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = EmailSink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            string mailSubject = EmailConnectionInfo.DefaultSubject)
        {
            if (connectionInfo == null) throw new ArgumentNullException("connectionInfo");
            if (textFormatter == null) throw new ArgumentNullException("textFormatter");

            ITextFormatter mailSubjectFormatter = new MessageTemplateTextFormatter(mailSubject, null);

            var defaultedPeriod = period ?? EmailSink.DefaultPeriod;

            return loggerConfiguration.Sink(
                new EmailSink(connectionInfo, batchPostingLimit, defaultedPeriod, textFormatter, mailSubjectFormatter),
                restrictedToMinimumLevel);
        }
    }
}
