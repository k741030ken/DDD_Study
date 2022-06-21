using System.Reflection;

namespace PXGo.Study.Infrastructure.Extensions;

public static class EnumExtension
{
    public static string GetResultCode(this Enum @enum)
    {
        var atr = GetAttribute<PXGoResultCodeAttribute>(@enum);

        if (atr != null)
        {
            return atr.ResultCode;
        }

        return PXGoDefaultValue.DEFAULT_ERROR_CODE;
    }

    public static string GetResultMessage(this Enum @enum)
    {
        var atr = GetAttribute<PXGoResultMessageAttribute>(@enum);
        if (atr != null)
        {
            return atr.ResulteMessage;
        }

        return PXGoDefaultValue.DEFAULT_ERROR_MESSAGE;
    }

    private static T GetAttribute<T>(Enum @enum) where T : Attribute
    {
        var fi = @enum.GetType().GetField(@enum.ToString());
        var attribute = fi.GetCustomAttribute(typeof(T), false)
            as T;
        return attribute;
    }

}
