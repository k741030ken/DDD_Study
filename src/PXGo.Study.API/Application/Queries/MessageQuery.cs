using MediatR;
using PXGo.Study.API.ViewModels;

namespace PXGo.Study.API.Application.Queries;

public class MessageQuery : IRequest<List<MessageVo>>
{
    public int TypeOne { get; set; }
}
