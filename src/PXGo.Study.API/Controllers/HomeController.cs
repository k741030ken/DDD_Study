using MediatR;
using Microsoft.AspNetCore.Mvc;
using PXGo.Study.API.Application.Commands.Message;
using PXGo.Study.API.Application.Queries;
using PXGo.Study.API.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace PXGo.Study.API.Controllers;

[ApiController]
public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMediator _mediator;

    public HomeController(ILogger<HomeController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [Route("/service/1.0/study/get_messages")]
    [SwaggerOperation(Summary = "取得 Messages", Tags = new[] { "FromQuery" })]
    [ProducesResponseType(typeof(PXGoResponse<List<MessageVo>>), 200)]
    public async Task<IActionResult> GetMessages([FromQuery] MessageQuery query, CancellationToken cancellationToken)
    {
        return Ok(new PXGoResponse<List<MessageVo>>(await _mediator.Send(query, cancellationToken)));
    }

    [HttpPost]
    [Route("/service/1.0/study/add_messages")]
    [SwaggerOperation(Summary = "新增 Messages", Tags = new[] { "FromBody" })]
    [ProducesResponseType(typeof(PXGoResponse<bool>), 200)]
    public async Task<IActionResult> AddMessages([FromBody]AddMessageCommand command, CancellationToken cancellationToken)
    {
        return Ok(new PXGoResponse<bool>(await _mediator.Send(command, cancellationToken)));
    }

    [HttpPost]
    [Route("/service/1.0/study/update_messages")]
    [SwaggerOperation(Summary = "修改 Messages", Tags = new[] { "FromBody" })]
    [ProducesResponseType(typeof(PXGoResponse<bool>), 200)]
    public async Task<IActionResult> UpdateMessages([FromBody] UpdateMessageCommand command, CancellationToken cancellationToken)
    {
        return Ok(new PXGoResponse<bool>(await _mediator.Send(command, cancellationToken)));
    }

    [HttpPost]
    [Route("/service/1.0/study/delete_messages")]
    [SwaggerOperation(Summary = "刪除 Messages", Tags = new[] { "FromBody" })]
    [ProducesResponseType(typeof(PXGoResponse<bool>), 200)]
    public async Task<IActionResult> DeleteMessages([FromBody] DeleteMessageCommand command, CancellationToken cancellationToken)
    {
        return Ok(new PXGoResponse<bool>(await _mediator.Send(command, cancellationToken)));
    }
}
