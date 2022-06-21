namespace PXGo.Study.Infrastructure.Extensions;

[AttributeUsage(AttributeTargets.Field)]
public class PXGoResultCodeAttribute : Attribute
{
    public PXGoResultCodeAttribute(string code)
    {
        ResultCode = code;
    }

    public string ResultCode { get; set; }
}