using MediatR;
using MediatR.Pipeline;

namespace PXGo.Study.API.Application.Behaviors;

/// <summary>
/// 針對MediatR的異常處理
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ExceptionBehavior<TRequest, TResponse> : IRequestExceptionHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionBehavior<TRequest, TResponse>> _logger;

    public ExceptionBehavior(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task Handle(TRequest request, Exception ex, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        _logger.LogError($"Handling exception:{request.GetType().FullName}, ex:{ex.Message}, {ex} ----- State:{state.Response}");
    }
}
