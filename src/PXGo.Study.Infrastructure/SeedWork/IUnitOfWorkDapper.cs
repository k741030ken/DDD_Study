using System.Data;

namespace PXGo.Study.Infrastructure.SeedWork;

/// <summary>
/// Dapper : 工作單位介面
/// 實現IDisposable接口進行手動的垃圾回收
/// </summary>
public interface IUnitOfWorkDapper : IDisposable
{
    /// <summary>
    /// Get current db connection - Master
    /// </summary>
    IDbConnection Master { get; }

    /// <summary>
    /// Get current db connection - Slave
    /// </summary>
    IDbConnection Slave { get; }

    /// <summary>
    /// Get current db transaction
    /// </summary>
    IDbTransaction Transaction { get; }

    /// <summary>
    /// Begin transaction 
    /// </summary>
    void BeginTransaction();

    /// <summary>
    /// Commit data changes.
    /// </summary>
    /// <returns></returns>
    void Commit();

    /// <summary>
    /// Rollback change datas.
    /// </summary>
    /// <returns></returns>
    void RollBack();
}
