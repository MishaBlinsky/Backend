using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using Backend.DAL.Models;

public class UnitOfWork(IOptions<DbSettings> dbSettings) : IDisposable
{
    private NpgsqlConnection _connection;
    public async Task<NpgsqlConnection> GetConnection(CancellationToken token)
    {
        if (_connection is not null)
        {
            return _connection;
        }
        var dataSource = new NpgsqlDataSourceBuilder(dbSettings.Value.ConnectionString);
        // Регистрируем все композитные типы
        dataSource.MapComposite<V1OrderDal>("v1_order");
        dataSource.MapComposite<V1OrderItemDal>("v1_order_item");
        dataSource.MapComposite<V1AuditLogOrderDal>("v1_audit_log_order"); // Добавляем новый тип
        _connection = dataSource.Build().CreateConnection();
        _connection.StateChange += (sender, args) =>
        {
            if (args.CurrentState == ConnectionState.Closed)
                _connection = null;
        };
        await _connection.OpenAsync(token);
        return _connection;
    }
    public NpgsqlConnection Connection => _connection;
    public NpgsqlTransaction Transaction { get; private set; }
    public async ValueTask<NpgsqlTransaction> BeginTransactionAsync(CancellationToken token)
    {
        _connection ??= await GetConnection(token);
        Transaction = await _connection.BeginTransactionAsync(token);
        return Transaction;
    }
    public void Dispose()
    {
        Transaction?.Dispose();
        DisposeConnection();
        GC.SuppressFinalize(this);
    }
    ~UnitOfWork()
    {
        DisposeConnection();
    }
    private void DisposeConnection()
    {
        _connection?.Dispose();
        _connection = null;
    }
}