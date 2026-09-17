namespace TechfinChallenge.Infrastructure.Persistence;

using System.Data;
using Microsoft.Data.Sqlite;

public interface IDbConnectionFactory
{
    IDbConnection GetConnection();
}

public class DbConnectionFactory : IDbConnectionFactory, IDisposable
{
    private readonly SqliteConnection _connection;
    private bool _disposed;

    public DbConnectionFactory()
    {
        _connection = new SqliteConnection("Data Source=TechfinDb;Mode=Memory;Cache=Shared");
        _connection.Open();
        DatabaseInitializer.Initialize(_connection);
    }

    public IDbConnection GetConnection() => _connection;

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
