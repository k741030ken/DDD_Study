using PXGo.Study.Domain.SeedWork;

namespace PXGo.Study.Domain.AggregatesModel.MessageAggregate;

public interface IMessageRepository : IRepository<MessageEntity>
{
    MessageEntity Add(MessageEntity entity);
    void Update(MessageEntity entity);
    void Delete(MessageEntity entity);
    Task<MessageEntity> GetAsync(int id, CancellationToken cancellationToken);
}
