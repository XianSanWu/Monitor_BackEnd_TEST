using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Repository.Interfaces;
using System.Data.Common;
using Utilities.Utilities;
using Models.Enums;

namespace Repository.Implementations
{
    /// <summary>
    /// DB連線的共用類
    /// </summary>
    public sealed class DbHelper : IDbHelper, IDisposable, IAsyncDisposable
    {
        private readonly string? _dbConnString;
        private readonly DbConnection _conn;
        private readonly UnitOfWork _unitOfWork;

        public DbHelper(IConfiguration _config, DBConnectionEnum ConnectionType = DBConnectionEnum.DefaultConnection)
        {
            // 選擇不同的 DB 連線
            _dbConnString = ConnectionType switch
            {
                DBConnectionEnum.Cdp => _config.GetConnectionString(DBConnectionEnum.Cdp.ToString()),
                DBConnectionEnum.Mail_hunter => _config.GetConnectionString(DBConnectionEnum.Mail_hunter.ToString()),
                _ => _config.GetConnectionString(DBConnectionEnum.DefaultConnection.ToString())
            };

            // 解密連線字串
            var key = _config["EncryptionSettings:AESKey"]!;
            var iv = _config["EncryptionSettings:AESIV"]!;
            var dbConnString = CryptoUtil.Decrypt(Base64Util.Decode((_dbConnString ?? string.Empty)), key, iv);

            var sqlConnBuilder = new SqlConnectionStringBuilder(dbConnString)
            {
                Pooling = true,
                PoolBlockingPeriod = PoolBlockingPeriod.NeverBlock,
                ConnectRetryCount = 3,
                ConnectRetryInterval = 10,  // Seconds
                ConnectTimeout = 300
            };

            _conn = new SqlConnection(sqlConnBuilder.ConnectionString);
            _unitOfWork = new UnitOfWork(_conn);
        }

        public UnitOfWork UnitOfWork => _unitOfWork;
        public DbConnection GetDbConnection() => _conn;

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            if (_conn?.State != System.Data.ConnectionState.Closed)
                _conn?.Close();
            _conn?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await _unitOfWork.DisposeAsync();
            if (_conn is not null)
            {
                if (_conn.State != System.Data.ConnectionState.Closed)
                    await _conn.CloseAsync();
                await _conn.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }
    }

    public class UnitOfWork : IUnitOfWork, IDisposable, IAsyncDisposable
    {
        private readonly Guid _id = Guid.NewGuid();
        private readonly DbConnection? _connection;
        private DbTransaction? _transaction;

        public UnitOfWork(DbConnection? connection = null) => _connection = connection;
        public DbConnection Connection => _connection ?? throw new InvalidOperationException("Connection is null.");
        public DbTransaction? Transaction => _transaction;
        Guid IUnitOfWork.Id => _id;

        public void Begin()
        {
            if (_connection == null)
                throw new InvalidOperationException("Cannot begin transaction: Connection is null.");
            if (_connection.State != System.Data.ConnectionState.Open)
                _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction is null.");
            await _transaction.CommitAsync().ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction is null.");
            await _transaction.RollbackAsync().ConfigureAwait(false);
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null;
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
