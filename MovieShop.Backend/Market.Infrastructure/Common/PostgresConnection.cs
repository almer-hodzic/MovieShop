using Npgsql;
namespace Market.Infrastructure.Common;

public static class PostgresConnection
{
    public static string Normalize(string value)
    {
        NpgsqlConnectionStringBuilder builder;
        if (value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(value);
            var user = uri.UserInfo.Split(':', 2);
            builder = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host, Port = uri.Port > 0 ? uri.Port : 5432,
                Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
                Username = Uri.UnescapeDataString(user[0]),
                Password = user.Length > 1 ? Uri.UnescapeDataString(user[1]) : "",
                SslMode = SslMode.VerifyFull
            };
        }
        else builder = new NpgsqlConnectionStringBuilder(value);
        if (builder.Host?.EndsWith(".neon.tech", StringComparison.OrdinalIgnoreCase) == true)
            builder.SslMode = SslMode.VerifyFull;
        builder.MaxPoolSize = 10;
        builder.MinPoolSize = 0;
        builder.ConnectionIdleLifetime = 60;
        return builder.ConnectionString;
    }
}
