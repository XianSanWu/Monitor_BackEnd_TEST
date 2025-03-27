using Services.Implementations;
using System.Data.Common;

namespace Services.Interfaces
{
    /// <summary>
    /// IDbHelper
    /// </summary>
    public interface IDbHelper : IDisposable
    {
        UnitOfWork UnitOfWork { get; }

        DbConnection GetDbConnection();
    }

    /// <summary>
    /// IUnitOfWork
    /// </summary>    
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        DbConnection Connection { get; }
        DbTransaction? Transaction { get; }
        void Begin();
        //void Commit();
        //void Rollback();
        //Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();

    }
}
