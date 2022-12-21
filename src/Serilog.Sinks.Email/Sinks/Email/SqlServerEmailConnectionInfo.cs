using System.ComponentModel;

namespace Serilog.Sinks.Email;

/// <summary>
/// Connection information for SqlServer msdb.dbo.sp_send_dbmail for use by the Email sink.
/// </summary>
public class SqlServerEmailConnectionInfo : IEmailConnectionInfo
{
    /// <summary>
    /// Gets or sets SqlServer connection string
    /// </summary>
    public string SqlConnectionString { get; set; }

    /// <summary>
    /// Gets or sets SqlServer mail profile name
    /// </summary>
    public string MailProfileName { get; set; }

    /// <summary>
    /// The email address emails will be sent from.
    /// </summary>
    public string FromEmail { get; set; }

    /// <summary>
    /// The email address(es) emails will be sent to. Accepts multiple email addresses separated by comma or semicolon.
    /// </summary>
    public string ToEmail { get; set; }

    /// <summary>
    /// The subject to use for the email, this can be a template.
    /// </summary>
    [DefaultValue(EmailConnectionInfo.DefaultSubject)]
    public string EmailSubject { get; set; }

    /// <summary>
    /// Sets whether the body contents of the email is HTML. Defaults to false.
    /// </summary>
    public bool IsBodyHtml { get; set; }

    public virtual IEmailTransport CreateEmailTransport()
    {
        return new SqlServerEmailTransport(MailProfileName, SqlConnectionString);
    }
}
