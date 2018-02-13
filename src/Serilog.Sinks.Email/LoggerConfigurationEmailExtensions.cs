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
using System.ComponentModel;

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
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        [Obsolete("New code should not be compiled against this obsolete overload"), EditorBrowsable(EditorBrowsableState.Never)]
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string outputTemplate,
            LogEventLevel restrictedToMinimumLevel,
            int batchPostingLimit,
            TimeSpan? period,
            IFormatProvider formatProvider,
            string mailSubject)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (fromEmail == null) throw new ArgumentNullException(nameof(fromEmail));
            if (toEmail == null) throw new ArgumentNullException(nameof(toEmail));

            var eventFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var subjectFormatter = new MessageTemplateTextFormatter(mailSubject, formatProvider);

            return BuildSink(loggerConfiguration, fromEmail, toEmail, eventFormatter, subjectFormatter, null, EmailConnectionInfo.DefaultPort,
                null, restrictedToMinimumLevel, batchPostingLimit, period);
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
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        [Obsolete("New code should not be compiled against this obsolete overload"), EditorBrowsable(EditorBrowsableState.Never)]
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string mailServer,
            ICredentialsByHost networkCredential,
            string outputTemplate,
            LogEventLevel restrictedToMinimumLevel,
            int batchPostingLimit,
            TimeSpan? period,
            IFormatProvider formatProvider,
            string mailSubject)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (fromEmail == null) throw new ArgumentNullException(nameof(fromEmail));
            if (toEmail == null) throw new ArgumentNullException(nameof(toEmail));

            var eventFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var subjectFormatter = new MessageTemplateTextFormatter(mailSubject, formatProvider);

            return BuildSink(loggerConfiguration, fromEmail, toEmail, eventFormatter, subjectFormatter, mailServer,
                EmailConnectionInfo.DefaultPort, networkCredential, restrictedToMinimumLevel, batchPostingLimit, period);
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
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            IEnumerable<string> toEmails,
            string mailServer,
            ICredentialsByHost networkCredential,
            string outputTemplate,
            LogEventLevel restrictedToMinimumLevel,
            int batchPostingLimit,
            TimeSpan? period,
            IFormatProvider formatProvider,
            string mailSubject)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (fromEmail == null) throw new ArgumentNullException(nameof(fromEmail));
            if (toEmails == null) throw new ArgumentNullException(nameof(toEmails));

            var eventFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var subjectFormatter = new MessageTemplateTextFormatter(mailSubject, formatProvider);

            return BuildSink(loggerConfiguration, fromEmail, String.Join(";", toEmails), eventFormatter, subjectFormatter, mailServer,
                EmailConnectionInfo.DefaultPort, networkCredential, restrictedToMinimumLevel, batchPostingLimit, period);
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
        [Obsolete("New code should not be compiled against this obsolete overload"), EditorBrowsable(EditorBrowsableState.Never)]
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
            if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));

            if (!string.IsNullOrEmpty(connectionInfo.EmailSubject))
            {
                mailSubject = connectionInfo.EmailSubject;
            }

            var eventFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var subjectFormatter = new MessageTemplateTextFormatter(mailSubject, formatProvider);

            return BuildSink(loggerConfiguration, connectionInfo.FromEmail, connectionInfo.ToEmail, eventFormatter, subjectFormatter,
                connectionInfo.MailServer, connectionInfo.Port, connectionInfo.NetworkCredentials, restrictedToMinimumLevel,
                batchPostingLimit, period);
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
        [Obsolete("New code should not be compiled against this obsolete overload"), EditorBrowsable(EditorBrowsableState.Never)]
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            EmailConnectionInfo connectionInfo,
            ITextFormatter textFormatter,
            LogEventLevel restrictedToMinimumLevel,
            int batchPostingLimit,
            TimeSpan? period,
            string mailSubject)
        {
            if (connectionInfo == null) throw new ArgumentNullException(nameof(connectionInfo));
            if (textFormatter == null) throw new ArgumentNullException(nameof(textFormatter));

            var subjectFormatter = new MessageTemplateTextFormatter(mailSubject, null);

            return BuildSink(loggerConfiguration, connectionInfo.FromEmail, connectionInfo.ToEmail, textFormatter, subjectFormatter,
                connectionInfo.MailServer, connectionInfo.Port, connectionInfo.NetworkCredentials, restrictedToMinimumLevel,
                batchPostingLimit, period);
        }

        /// <summary>
        /// Adds a sink that sends log events via email.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmail">The email address emails will be sent to</param>
        /// <param name="mailSubject">The subject, can be a plain string or a template such as {Timestamp} [{Level}] occurred.</param>
        /// <param name="outputTemplate">A message template describing the format used to write to the sink.
        /// the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".</param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <param name="mailServer">The SMTP server; if <i>mailServer</i> == <i>null</i> the settings from App.config or Web.config will be used.</param>
        /// <param name="smtpPort">The SMTP server port</param>
        /// <param name="networkCredential">The network credentials to use to authenticate with mailServer</param>
        /// <param name="minimumLogLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single email.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        public static LoggerConfiguration Email(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            string mailSubject = EmailConnectionInfo.DefaultSubject,
            string outputTemplate = DefaultOutputTemplate,
            string mailServer = null,
            int smtpPort = EmailConnectionInfo.DefaultPort,
            ICredentialsByHost networkCredential = null,
            LogEventLevel minimumLogLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null,
            int batchSizeLimit = EmailSink.DefaultBatchPostingLimit,
            TimeSpan? period = null)
        {
            if (fromEmail == null) throw new ArgumentNullException(nameof(fromEmail));
            if (toEmail == null) throw new ArgumentNullException(nameof(toEmail));

            var eventFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            var subjectFormatter = new MessageTemplateTextFormatter(mailSubject, formatProvider);

            return BuildSink(loggerConfiguration, fromEmail, toEmail, eventFormatter, subjectFormatter, mailServer, smtpPort,
                networkCredential, minimumLogLevel, batchSizeLimit, period);
        }

        /// <summary>
        /// This method is responsible for adapting all the above overloads to exactly what is required by <see cref="EmailSink"/>.
        /// </summary>
        /// <param name="loggerConfiguration">The logger configuration.</param>
        /// <param name="fromEmail">The email address emails will be sent from</param>
        /// <param name="toEmail">The email address emails will be sent to</param>
        /// <param name="mailServer">The SMTP server; if <i>mailServer</i> == <i>null</i> the settings from App.config or Web.config will be used.</param>
        /// <param name="smtpPort">The SMTP server port</param>
        /// <param name="networkCredential">The network credentials to use to authenticate with mailServer</param>
        /// <param name="minimumLogLevel">The minimum log event level required in order to write an event to the sink.</param>
        /// <param name="batchSizeLimit">The maximum number of events to include in a single email.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="eventFormatter">ITextFormatter implementation to write log entry to email body.</param>
        /// <param name="subjectFormatter">ITextFormatter implementation to write log entry to email subject.</param>
        /// <returns>Logger configuration, allowing configuration to continue.</returns>
        /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
        private static LoggerConfiguration BuildSink(
            this LoggerSinkConfiguration loggerConfiguration,
            string fromEmail,
            string toEmail,
            ITextFormatter eventFormatter,
            ITextFormatter subjectFormatter,
            string mailServer,
            int smtpPort,
            ICredentialsByHost networkCredential,
            LogEventLevel minimumLogLevel,
            int batchSizeLimit,
            TimeSpan? period)
        {
            if (fromEmail == null) throw new ArgumentNullException(nameof(fromEmail));
            if (toEmail == null) throw new ArgumentNullException(nameof(toEmail));
            if (eventFormatter == null) throw new ArgumentNullException(nameof(eventFormatter));
            if (subjectFormatter == null) throw new ArgumentNullException(nameof(subjectFormatter));

            var connectionInfo = new EmailConnectionInfo
            {
                FromEmail = fromEmail,
                ToEmail = toEmail,
                MailServer = mailServer,
                Port = smtpPort,
                NetworkCredentials = networkCredential
            };

            var defaultPeriod = period ?? EmailSink.DefaultPeriod;

            return loggerConfiguration.Sink(
                new EmailSink(connectionInfo, batchSizeLimit, defaultPeriod, eventFormatter, subjectFormatter), minimumLogLevel);
        }
    }
}
