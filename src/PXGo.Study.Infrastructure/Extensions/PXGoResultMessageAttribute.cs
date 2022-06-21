namespace PXGo.Study.Infrastructure.Extensions;

[AttributeUsage(AttributeTargets.Field)]
public class PXGoResultMessageAttribute : Attribute
{
    public PXGoResultMessageAttribute(string message)
    {
        ResulteMessage = message;
    }

    public string ResulteMessage { get; set; }
}