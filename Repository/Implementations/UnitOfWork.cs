using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Enums;
using Repository.Interfaces;
using System.Data.Common;
using Utilities.Utilities;

namespace Repository.Implementations
{
    /// <summary>
    /// DB連線的共用類
    /// </summary>
    public sealed class DbHelper : IDbHelper
    {
        private readonly string? _dbConnString;
        private readonly DbConnection _conn;
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// 選擇 appsettings.json 中 不同的DB連線
        /// </summary>
        /// <param name="ConnectionType"></param>
        public DbHelper(IConfiguration _config, DBConnectionEnum ConnectionType = DBConnectionEnum.DefaultConnection)
        {
            switch (ConnectionType)
            {
                case DBConnectionEnum.Cdp:
                    {
                        _dbConnString = _config.GetConnectionString(DBConnectionEnum.Cdp.ToString());
                    }
                    break;
                default:
                    {
                        _dbConnString = _config.GetConnectionString(DBConnectionEnum.DefaultConnection.ToString());
                    }
                    break;
            }

            var dbConnString = (_dbConnString ?? "");
            var key = "";
            var iv = "";
            try
            {
                key = _config["EncryptionSettings:AESKey"]!;
                iv = _config["EncryptionSettings:AESIV"]!;
                //key = _config.GetValue<string>("EncryptionSettings:AESKey");
                //iv = _config.GetValue<string>("EncryptionSettings:AESIV");

                dbConnString = Base64Util.Decode(dbConnString);
                dbConnString = CryptoHelper.Decrypt(dbConnString, key, iv);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(key);
                Console.WriteLine(iv);
                throw new InvalidOperationException("連線字串解密失敗，請檢查 Key/IV 或格式", ex);
            }

            #region 建立和管理 SqlConnection 類別使用之連接字串的內容          
            SqlConnectionStringBuilder SqlConnBuilder = new(dbConnString)
            {
                Pooling = true,
                PoolBlockingPeriod = PoolBlockingPeriod.NeverBlock,
                ConnectRetryCount = 3,
                ConnectRetryInterval = 10,  //Seconds
                ConnectTimeout = 300
            };
            #endregion

            _conn = new SqlConnection(SqlConnBuilder.ConnectionString);  // 使用 Microsoft.Data.SqlClient.SqlConnection
            _unitOfWork = new UnitOfWork(_conn);
        }

        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public DbConnection GetDbConnection()
        {
            return _conn;
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _conn?.Dispose();
        }
    }

    public class UnitOfWork(DbConnection? connection = null) : IUnitOfWork
    {
        private readonly Guid _id = Guid.NewGuid();
        private DbTransaction? _transaction;
        public DbConnection Connection => connection ?? throw new InvalidOperationException("Connection is null.");
        public DbTransaction? Transaction => _transaction;

        Guid IUnitOfWork.Id
        {
            get { return _id; }
        }

        public void Begin()
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Cannot begin transaction: Connection is null.");
            }

            connection?.Open();
            _transaction = connection?.BeginTransaction();
        }
        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction is null.");
            }
            await _transaction.CommitAsync().ConfigureAwait(false);
        }
        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction is null.");
            }
            await _transaction.RollbackAsync().ConfigureAwait(false);
        }
        public void Dispose()
        {
            _transaction?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
