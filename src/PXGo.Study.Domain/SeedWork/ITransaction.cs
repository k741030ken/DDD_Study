using Microsoft.EntityFrameworkCore.Storage;

namespace PXGo.Study.Domain.SeedWork;

/// <summary>
/// 事務介面
/// </summary>
public interface ITransaction
{
	IDbContextTransaction GetCurrentTransaction();

	bool HasActiveTransaction { get; }

	Task<IDbContextTransaction> BeginTransactionAsync();

	Task CommitTransactionAsync(IDbContextTransaction transaction);

	void RollbackTransaction();
}
