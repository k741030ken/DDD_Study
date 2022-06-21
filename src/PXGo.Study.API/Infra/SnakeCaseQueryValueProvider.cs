using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Text;

namespace PXGo.Study.API.Infra;


/// <summary>
/// QueryString 參數選擇使用SnakeCase 的形式
/// </summary>
public class SnakeCaseQueryValueProvider : QueryStringValueProvider
{
    public SnakeCaseQueryValueProvider(BindingSource bindingSource,IQueryCollection values, CultureInfo culture) : base(bindingSource, values, culture)
    {
    }

    public override bool ContainsPrefix(string prefix)
    {
        return base.ContainsPrefix(prefix.ToSnakeCase());
    }

    public override ValueProviderResult GetValue(string key)
    {
        return base.GetValue(key.ToSnakeCase());
    }
}

public class SnakeCaseQueryValueProviderFactory : IValueProviderFactory
{
    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        var valueProvider = new SnakeCaseQueryValueProvider(
            BindingSource.Query,
            context.ActionContext.HttpContext.Request.Query,
            CultureInfo.CurrentCulture);
        context.ValueProviders.Add(valueProvider);
        return Task.CompletedTask;
    }
}

public static class StringExtensions
{
    public static string ToSnakeCase(this string str)
    {
        if (str == string.Empty)
            return str;
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(char.ToLower(str[0]));
        for (int i = 1; i < str.Length; i++)
        {
            if (Char.IsUpper(str[i]))
                stringBuilder.Append($"_");
            stringBuilder.Append(Char.ToLower(str[i]));
        }
        return stringBuilder.ToString();
    }
}

public class SnakeCaseQueryParametersApiDescriptionProvider : IApiDescriptionProvider
{
    public int Order => 1;

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (var parameter in context.Results.SelectMany(x => x.ParameterDescriptions).Where(x => x.Source.Id == "Query" || x.Source.Id == "ModelBinding"))
            parameter.Name = parameter.Name.ToSnakeCase();
    }
}

public class SnakecasingParameOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();
        else
        {
            foreach (var item in operation.Parameters)
                item.Name = item.Name.ToSnakeCase();
        }
    }
}
