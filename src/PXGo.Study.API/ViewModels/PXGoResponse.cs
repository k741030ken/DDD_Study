using PXGo.Study.API.Application.Models;
using PXGo.Study.Infrastructure.Enums;
using PXGo.Study.Infrastructure.Extensions;
using System.Text.Json.Serialization;

namespace PXGo.Study.API.ViewModels;

public class PXGoResponse : PXGoResponse<object>
{

    public void Set(CommandResult result)
    {
        if (result.Code == PXGoDefaultValue.SUCCESS_CODE)
        {
            base.SetSuccess(result.Data);
        }
        else
        {
            base.SetFailure(result.Code, result.Message);
        }
    }

    public void SetFailure(ExceptionType exceptionType, string warnning = "")
    {
        this.SetFailure(exceptionType.GetResultCode(), exceptionType.GetResultMessage(), warnning);
    }
}

public class PXGoResponse<T>
{
    public PXGoResponse()
    {

    }

    public PXGoResponse(T data)
    {
        Code = PXGoDefaultValue.SUCCESS_CODE;
        Message = PXGoDefaultValue.SUCCESS_MESSAGE;
        SrvCode = PXGoDefaultValue.SERVER_CODE;
        Warnning = "";
        Data = data;
    }

    /// <summary>
    /// 錯誤代碼(正常回 0000)
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; }

    /// <summary>
    /// 服務代碼
    /// </summary>
    [JsonPropertyName("srv")]
    public string SrvCode { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; private set; }


    /// <summary>
    /// 錯誤訊息
    /// </summary>
    [JsonPropertyName("warnning")]
    public string Warnning { get; private set; }

    /// <summary>
    /// Response Data
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; private set; }

    public void SetSuccess()
    {
        Code = PXGoDefaultValue.SUCCESS_CODE;
        Message = PXGoDefaultValue.SUCCESS_MESSAGE;
        SrvCode = PXGoDefaultValue.SERVER_CODE;
        Warnning = "";
    }

    public void SetSuccess(T data)
    {
        Code = PXGoDefaultValue.SUCCESS_CODE;
        Message = PXGoDefaultValue.SUCCESS_MESSAGE;
        SrvCode = PXGoDefaultValue.SERVER_CODE;
        Warnning = "";
        Data = data;
    }

    public void SetFailure(string code, string message, string warnning = "")
    {
        Code = code;
        Message = message;
        Warnning = warnning;
        SrvCode = PXGoDefaultValue.SERVER_CODE;
    }

    public void SetUnCatchException()
    {
        Code = PXGoDefaultValue.DEFAULT_ERROR_CODE;
        Message = PXGoDefaultValue.DEFAULT_ERROR_MESSAGE;
        SrvCode = PXGoDefaultValue.SERVER_CODE;
    }
}