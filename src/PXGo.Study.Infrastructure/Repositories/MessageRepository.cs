using Microsoft.EntityFrameworkCore;
using PXGo.Study.Domain.AggregatesModel.MessageAggregate;
using PXGo.Study.Domain.SeedWork;

namespace PXGo.Study.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly DBContext _context;

    public MessageRepository(DBContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public MessageEntity Add(MessageEntity entity)
    {
        return _context.Messages.Add(entity).Entity;
    }

    public void Update(MessageEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(MessageEntity entity)
    {
        _context.Entry(entity).State = EntityState.Deleted;
    }

    public async Task<MessageEntity> GetAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Messages.Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

}
