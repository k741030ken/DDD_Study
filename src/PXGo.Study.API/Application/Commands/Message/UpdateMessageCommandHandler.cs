using MediatR;
using PXGo.Study.Domain.AggregatesModel.MessageAggregate;

namespace PXGo.Study.API.Application.Commands.Message;

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, bool>
{
    private readonly ILogger<UpdateMessageCommandHandler> _logger;
    private readonly IMessageRepository _messageRepo;

    public UpdateMessageCommandHandler(ILogger<UpdateMessageCommandHandler> logger, IMessageRepository messageRepo)
    {
        _logger = _logger;
        _messageRepo = messageRepo;
    }

    public async Task<bool> Handle(UpdateMessageCommand request, CancellationToken cancellation)
    {
        try
        {
            MessageEntity getData = await _messageRepo.GetAsync(request.Id, cancellation);
            if (getData == null)
                return false;
            getData.UpdateMessage(request.TypeOne, request.TypeTwo, request.Content);
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
