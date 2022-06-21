using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using PXGo.Study.Domain.AggregatesModel.MessageAggregate;
using PXGo.Study.Domain.SeedWork;
using PXGo.Study.Infrastructure.EntityConfigs;
using PXGo.Study.Infrastructure.Extensions;
using System.Data;

namespace PXGo.Study.Infrastructure;

/// <summary>
/// DbContext 實例代表工作單位和儲存機制模式的組合，讓它可以用來從資料庫查詢，然後將變更群組在一起，然後再以一個單位寫回存放區
/// </summary>
public class DBContext : DbContext, ITransaction, IUnitOfWork
{
    public const string DEFAULT_SCHEMA = "sample"; //DB SCHEMA 名稱

    private IDbContextTransaction _currentTransaction; //當前事務
    private readonly IMediator _mediator;
    private readonly ILogger<DBContext> _logger;

    /// <summary>
	/// 公開方法，返回當前私有事務物件
	/// </summary>
	/// <returns></returns>
	public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

    /// <summary>
    /// 當前事務是否開啟
    /// </summary>
    public bool HasActiveTransaction => _currentTransaction == null;

    /// <summary>
    /// DBContext 的 建構式
    /// </summary>
    /// <param name="options"></param>
    public DBContext(DbContextOptions<DBContext> options) : base(options) { }

    /// <summary>
    /// DBContext 的 建構式
    /// </summary>
    /// <param name="options"></param>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DBContext(DbContextOptions<DBContext> options, IMediator mediator, ILogger<DBContext> logger) : base(options)
    {
        _logger = logger;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /*在 DbConent 宣告 DbSet<TEntity>，TEntity 即會被包含在 Model 範圍，TEntity 屬性涉及的其他型別也會被包含進來。
     * OnModelCreating() modelBuilder.Entity<TMoreEntity>() 亦有將 TMoreEntity 納入的效果。*/

    #region 宣告 DbSet<TEntity> 命名要為複數

    public DbSet<MessageEntity> Messages { get; set; }

    #endregion

    /// <summary>
    /// Fluent API 注入
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageEntityConfigs());
    }

    /// <summary>
    /// 儲存實體變更
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _mediator.DispatchDomainEventsAsync(this);  // Mediator 擴充 MediatorExtension
            return await base.SaveChangesAsync(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// 開啟事務
    /// </summary>
    /// <returns></returns>
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            return null;
        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted); //Pomelo.EntityFrameworkCore.MySql 套件
        return _currentTransaction;
    }

    /// <summary>
    /// 提交事務
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");
        try
        {
            transaction.Commit();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// 回滾事務
    /// </summary>
    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
