using PXGo.Study.Infrastructure.Extensions;

namespace PXGo.Study.Infrastructure.Enums;

/// <summary>
/// ResultCode 列舉
/// </summary>
public enum ExceptionType
{
    // 成功
    [PXGoResultCode("0000")]
    [PXGoResultMessage("成功")]
    Success = 0,

    [PXGoResultCode("9999")]
    [PXGoResultMessage("PXEC System Error")]
    DefaultError = 9999,

    #region 一般錯誤
    /*
     * 錯誤訊息編號
     * 2151~2250
     */
    [PXGoResultCode("2151")]
    [PXGoResultMessage("欄位資料驗證錯誤")]
    RequestParameter = 2151,

    [PXGoResultCode("2152")]
    [PXGoResultMessage("Service沒有使用權限")]
    ServiceNoPermission = 2152,

    [PXGoResultCode("2153")]
    [PXGoResultMessage("沒有使用權限")]
    NoPermission = 2153,

    [PXGoResultCode("2154")]
    [PXGoResultMessage("Token不合法或以逾期，請Refresh Token。")]
    TokenInvalid = 2154,

    [PXGoResultCode("2155")]
    [PXGoResultMessage("Refresh Token 不合法，請重新登入")]
    TokenNotFound = 2155,

    [PXGoResultCode("2156")]
    [PXGoResultMessage("Refresh Token 已失效，請重新登入")]
    TokenExchanged = 2156,

    [PXGoResultCode("2157")]
    [PXGoResultMessage("驗證失敗，請重新登入")]
    GetOuterIdFail = 2157,

    [PXGoResultCode("2158")]
    [PXGoResultMessage("驗證會員失敗，請重新登入")]
    GetUserIdFail = 2158,

    [PXGoResultCode("2159")]
    [PXGoResultMessage("驗證使用者失敗，請重新登入")]
    GetManagerUserIdFail = 2159,

    [PXGoResultCode("2160")]
    [PXGoResultMessage("請重新登入PXPay")]
    GetPXPayOuterIdFail = 2160,

    [PXGoResultCode("2161")]
    [PXGoResultMessage("無法取得登入Token")]
    GetJwtTokenFail = 2161,


    [PXGoResultCode("2162")]
    [PXGoResultMessage("資料寫入錯誤")]
    SaveChangesError = 2162,

    [PXGoResultCode("2163")]
    [PXGoResultMessage("Exception錯誤")]
    IsException = 2163,

    [PXGoResultCode("2164")]
    [PXGoResultMessage("資料未更新")]
    UpdateNoChange = 2164,

    [PXGoResultCode("2165")]
    [PXGoResultMessage("查無此資料")]
    NoData = 2165,

    [PXGoResultCode("2166")]
    [PXGoResultMessage("查狀態無法更新資料")]
    TheStatusCanNotUpdate = 2166,

    [PXGoResultCode("2167")]
    [PXGoResultMessage("新增暫存產品失敗")]
    SaveTempProductFail = 2167,

    #endregion

}
