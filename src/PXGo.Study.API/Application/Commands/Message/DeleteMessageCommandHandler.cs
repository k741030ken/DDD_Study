using MediatR;
using PXGo.Study.Domain.AggregatesModel.MessageAggregate;

namespace PXGo.Study.API.Application.Commands.Message;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, bool>
{
    private readonly ILogger<DeleteMessageCommandHandler> _logger;
    private readonly IMessageRepository _messageRepo;

    public DeleteMessageCommandHandler(ILogger<DeleteMessageCommandHandler> logger, IMessageRepository messageRepo)
    {
        _logger = _logger;
        _messageRepo = messageRepo;
    }

    public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellation)
    {
        try
        {
            MessageEntity getData = await _messageRepo.GetAsync(request.Id, cancellation);
            if (getData == null)
                return false;
            _messageRepo.Delete(getData);
            if (!await _messageRepo.UnitOfWork.SaveEntitiesAsync(cancellation))
                return false;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}
