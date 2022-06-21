using MediatR;
using PXGo.Study.Domain.AggregatesModel.Enums;

namespace PXGo.Study.API.Application.Commands.Message;

public class UpdateMessageCommand : BaseCommand, IRequest<bool>
{
    public int Id { get; set; }

    public OneType TypeOne { get; set; }

    public int TypeTwo { get; set; }

    public string Content { get; set; }
}
