using PXGo.Study.Infrastructure.Enums;
using PXGo.Study.Infrastructure.Extensions;

namespace PXGo.Study.API.Application.Models;

public class CommandResult
{
    public string Code { get; set; }

    public object Data { get; set; }

    public string Srv { get; set; }

    public string Message { get; set; }

    public string Warnning { get; set; }

    public static CommandResult CreateSuccess<T>(T data) => new()
    {
        Code = PXGoDefaultValue.SUCCESS_CODE,
        Message = PXGoDefaultValue.SUCCESS_MESSAGE,
        Srv = PXGoDefaultValue.SERVER_CODE,
        Data = data
    };

    public static CommandResult CreateSuccess() => new()
    {
        Code = PXGoDefaultValue.SUCCESS_CODE,
        Message = PXGoDefaultValue.SUCCESS_MESSAGE,
        Srv = PXGoDefaultValue.SERVER_CODE,
    };

    public static CommandResult CreateError(ExceptionType exType, string message = "") => new()
    {
        Code = exType.GetResultCode(),
        Srv = PXGoDefaultValue.SERVER_CODE,
        Message = exType.GetResultMessage(),
        Warnning = message,
    };

    public static CommandResult CreateError(string code, string message)
    {
        return new CommandResult
        {
            Code = code,
            Srv = PXGoDefaultValue.SERVER_CODE,
            Message = message
        };
    }
}