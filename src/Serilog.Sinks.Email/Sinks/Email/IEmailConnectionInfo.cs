using System.ComponentModel;

namespace Serilog.Sinks.Email;

public interface IEmailConnectionInfo
{
    /// <summary>
    /// The email address emails will be sent from.
    /// </summary>
    string FromEmail { get; set; }

    /// <summary>
    /// The email address(es) emails will be sent to. Accepts multiple email addresses separated by comma or semicolon.
    /// </summary>
    string ToEmail { get; set; }

    /// <summary>
    /// The subject to use for the email, this can be a template.
    /// </summary>
    [DefaultValue(EmailConnectionInfo.DefaultSubject)]
    string EmailSubject { get; set; }

    /// <summary>
    /// Sets whether the body contents of the email is HTML. Defaults to false.
    /// </summary>
    bool IsBodyHtml { get; set; }

    IEmailTransport CreateEmailTransport();
}
