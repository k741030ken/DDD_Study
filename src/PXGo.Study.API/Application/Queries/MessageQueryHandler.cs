using AutoMapper;
using Dapper;
using MediatR;
using PXGo.Study.API.Application.Models;
using PXGo.Study.API.ViewModels;
using PXGo.Study.Infrastructure.SeedWork;

namespace PXGo.Study.API.Application.Queries;

public class MessageQueryHandler : IRequestHandler<MessageQuery, List<MessageVo>>
{
    private readonly IUnitOfWorkDapper _dapper;
    private readonly ILogger<MessageQueryHandler> _logger;
    private readonly IMapper _mapper;

    public MessageQueryHandler(IUnitOfWorkDapper dapper, ILogger<MessageQueryHandler> logger, IMapper mapper)
    {
        _dapper = dapper;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<MessageVo>> Handle(MessageQuery request, CancellationToken cancellationToken)
    {
        List<MessageVo> result = new();
        List<MessageModel> getData = new();
        DynamicParameters parameters = new();
        parameters.Add("@TypeOne", request.TypeOne);
        string sqlQuery = @"SELECT * FROM messages WHERE type_one=@TypeOne;";

        var multi = await _dapper.Slave.QueryMultipleAsync(new CommandDefinition(sqlQuery, parameters, cancellationToken: cancellationToken));
        if (multi == null)
            return result;
        getData = multi.Read<MessageModel>().ToList();
        result = _mapper.Map<List<MessageVo>>(getData); // DB資料合併到ViewModel可針對資料做格式化
        return result;
    }

}
