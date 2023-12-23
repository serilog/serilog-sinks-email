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
using System.ComponentModel;
using System.Net;
using MailKit.Security;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Serilog.Sinks.Email;

/// <summary>
/// Connection information for use by the Email sink.
/// </summary>
public sealed class EmailConnectionInfo
{
    /// <summary>
    /// The default port used by for SMTP transfer.
    /// </summary>
    const int DefaultPort = 25;

    /// <summary>
    /// The default subject used for email messages.
    /// </summary>
    public const string DefaultSubject = "Log Email";

    /// <summary>
    /// Constructs the <see cref="EmailConnectionInfo"/> with the default port and default email subject set.
    /// </summary>
    public EmailConnectionInfo()
    {
        Port = DefaultPort;
        EmailSubject = DefaultSubject;
        IsBodyHtml = false;
    }

    /// <summary>
    /// Gets or sets the credentials used for authentication.
    /// </summary>
    public ICredentialsByHost? NetworkCredentials { get; set; }

    /// <summary>
    /// Gets or sets the port used for the connection.
    /// Default value is 25.
    /// </summary>
    [DefaultValue(DefaultPort)]
    public int Port { get; set; }

    /// <summary>
    /// The email address emails will be sent from.
    /// </summary>
    public string? FromEmail { get; set; }

    /// <summary>
    /// The email address(es) emails will be sent to. Accepts multiple email addresses separated by comma or semicolon.
    /// </summary>
    public string? ToEmail { get; set; }

    /// <summary>
    /// The subject to use for the email, this can be a template.
    /// </summary>
    [DefaultValue(DefaultSubject)]
    public string EmailSubject { get; set; }

    /// <summary>
    /// Selects the <see cref="SecureSocketOptions.SslOnConnect"/> secure socket option. This is provided for
    /// backwards compatibility and may not be what your mail server needs; instead, set <see cref="SecureSocketOption"/>
    /// to either <see cref="SecureSocketOptions.SslOnConnect"/> or <see cref="SecureSocketOptions.StartTls"/> explicitly.
    /// </summary>
    [Obsolete("Choose a specific `SecureSocketOption` instead.")]
    public bool EnableSsl { get; set; }

    /// <summary>
    /// Choose the security applied to the SMTP connection. This enumeration type is supplied by MailKit; see the
    /// MailKit documentation for supported values.
    /// </summary>
    public SecureSocketOptions? SecureSocketOption { get; set; }

    /// <summary>
    /// Provides a method that validates server certificates.
    /// </summary>
    public System.Net.Security.RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }

    /// <summary>
    /// The SMTP email server to use.
    /// </summary>
    public string? MailServer { get; set; }

    /// <summary>
    /// Sets whether the body contents of the email is HTML. Defaults to false.
    /// </summary>
    public bool IsBodyHtml { get; set; }

    internal SecureSocketOptions GetSecureSocketOption()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        return SecureSocketOption ?? (EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
