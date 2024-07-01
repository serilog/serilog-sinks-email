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

using MailKit.Security;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.Collections.Generic;
using System.Net;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Serilog.Sinks.Email;

/// <summary>
/// Connection information for use by the Email sink.
/// </summary>
public sealed class EmailSinkOptions
{
    internal const int DefaultPort = 25;
    const string DefaultSubject = "Log Messages";
    const string DefaultBody = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";
    internal const SecureSocketOptions DefaultConnectionSecurity = SecureSocketOptions.Auto;

    /// <summary>
    /// Constructs an <see cref="EmailSinkOptions"/> with default options.
    /// </summary>
    public EmailSinkOptions()
    {
    }

    /// <summary>
    /// Constructs an <see cref="EmailSinkOptions"/> with specified options.
    /// </summary>
    /// <param name="from">The email address emails will be sent from.</param>
    /// <param name="to">The email address emails will be sent to. Multiple addresses can be separated
    /// with commas or semicolons.</param>
    /// <param name="host">The SMTP email server to use</param>
    /// <param name="port">Gets or sets the port used for the SMTP connection. The default is 25.</param>
    /// <param name="connectionSecurity">Choose the security applied to the SMTP connection. This enumeration type
    /// is supplied by MailKit; see <see cref="SecureSocketOptions"/> for supported values. The default is
    /// <see cref="SecureSocketOptions.Auto"/>.</param>
    /// <param name="credentials">The network credentials to use to authenticate with mailServer</param>
    /// <param name="subject">The <see cref="ITextFormatter"/> implementation to format email subjects. Specify
    /// <c>null</c> to use the default subject. Consider using <see cref="MessageTemplateTextFormatter"/> or
    /// <c>Serilog.Expressions</c> templates.</param>
    /// <param name="body">The <see cref="ITextFormatter"/> or <see cref="IBatchTextFormatter"/> implementation
    /// to write log entries to email. Specify <c>null</c> to use the default body. Consider using
    /// <see cref="MessageTemplateTextFormatter"/> or <c>Serilog.Expressions</c> templates.</param>
    /// <exception cref="ArgumentNullException">A required parameter is null.</exception>
    public EmailSinkOptions(
        string from,
        string to,
        string host,
        int port = DefaultPort,
        SecureSocketOptions connectionSecurity = DefaultConnectionSecurity,
        ICredentialsByHost? credentials = null,
        ITextFormatter? subject = null,
        ITextFormatter? body = null)
    {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (to == null) throw new ArgumentNullException(nameof(to));
        if (host == null) throw new ArgumentNullException(nameof(host));

        From = from;
        To = LoggerConfigurationEmailExtensions.SplitToAddresses(to);
        Host = host;
        Port = port;
        ConnectionSecurity = connectionSecurity;
        Credentials = credentials;

        if (subject != null)
        {
            Subject = subject;
        }

        if (body != null)
        {
            Body = body;
        }
    }

    /// <summary>
    /// The email address emails will be sent from.
    /// </summary>
    public string From { get; set; } = null!;

    /// <summary>
    /// The email address(es) emails will be sent to.
    /// </summary>
    public List<string> To { get; set; } = [];

    /// <summary>
    /// The SMTP email server to use.
    /// </summary>
    public string Host { get; set; } = null!;

    /// <summary>
    /// Gets or sets the port used for the SMTP connection. The default is 25.
    /// </summary>
    public int Port { get; set; } = DefaultPort;

    /// <summary>
    /// Gets or sets the credentials used for authentication.
    /// </summary>
    public ICredentialsByHost? Credentials { get; set; }

    /// <summary>
    /// The <see cref="ITextFormatter"/> implementation to format email subjects. Specify
    /// <c>null</c> to use the default subject. Consider using <see cref="MessageTemplateTextFormatter"/> or
    /// <c>Serilog.Expressions</c> templates.
    /// </summary>
    public ITextFormatter Subject { get; set; } = new MessageTemplateTextFormatter(DefaultSubject);

    /// <summary>
    /// The <see cref="ITextFormatter"/> or <see cref="IBatchTextFormatter"/> implementation
    /// to write log entries to email. Specify <c>null</c> to use the default body. Consider using
    /// <see cref="MessageTemplateTextFormatter"/> or <c>Serilog.Expressions</c> templates.
    /// </summary>
    public ITextFormatter Body { get; set; } = new MessageTemplateTextFormatter(DefaultBody);

    /// <summary>
    /// Sets whether the body contents of the email is HTML. Defaults to false.
    /// </summary>
    public bool IsBodyHtml { get; set; }

    /// <summary>
    /// Choose the security applied to the SMTP connection. This enumeration type is supplied by MailKit; see
    /// <see cref="SecureSocketOptions"/> for supported values. The default is
    /// <see cref="SecureSocketOptions.Auto"/>.
    /// </summary>
    public SecureSocketOptions ConnectionSecurity { get; set; } = DefaultConnectionSecurity;

    /// <summary>
    /// Provides a method that validates server certificates.
    /// </summary>
    public System.Net.Security.RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }
}
