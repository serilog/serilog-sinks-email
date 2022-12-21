using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Serilog.Sinks.Email;

class SqlServerEmailTransport : IEmailTransport
{
    private readonly string _profileName;
    private readonly Lazy<SqlConnection> _sqlConnection;

    public SqlServerEmailTransport(string profileName, string sqlConnectionString)
    {
        if (string.IsNullOrEmpty(profileName))
            throw new ArgumentException("Value cannot be null or empty.", nameof(profileName));
        if (string.IsNullOrEmpty(sqlConnectionString))
            throw new ArgumentException("Value cannot be null or empty.", nameof(sqlConnectionString));

        _profileName = profileName;
        _sqlConnection = new Lazy<SqlConnection>(() => new SqlConnection(sqlConnectionString));
    }

    public void Dispose()
    {
        if (_sqlConnection.IsValueCreated)
        {
            _sqlConnection.Value.Dispose();
        }
    }

    public Task SendMailAsync(EmailMessage emailMessage)
    {
        var sqlCommand = new SqlCommand("msdb.dbo.sp_send_dbmail", _sqlConnection.Value)
        {
            CommandType = CommandType.StoredProcedure
        };

        sqlCommand.Parameters.AddWithValue("profile_name", _profileName);
        sqlCommand.Parameters.AddWithValue("from_address", emailMessage.From);
        sqlCommand.Parameters.AddWithValue("recipients", emailMessage.To);
        sqlCommand.Parameters.AddWithValue("subject", emailMessage.Subject);
        sqlCommand.Parameters.AddWithValue("body", emailMessage.Body);
        sqlCommand.Parameters.AddWithValue("body_format", emailMessage.IsBodyHtml ? "HTML" : "TEXT");

        return sqlCommand.ExecuteNonQueryAsync();
    }
}
