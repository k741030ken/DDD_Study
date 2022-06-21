namespace PXGo.Study.Domain.SeedWork;

/// <summary>
/// EF : 工作單位介面
/// 實現IDisposable接口進行手動的垃圾回收
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
}
