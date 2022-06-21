using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using PXGo.Study.API.Application.Behaviors;
using PXGo.Study.API.Application.Commands;
using PXGo.Study.API.Application.Queries;
using PXGo.Study.API.Infra;
using PXGo.Study.Domain.AggregatesModel.MessageAggregate;
using PXGo.Study.Infrastructure;
using PXGo.Study.Infrastructure.Repositories;
using PXGo.Study.Infrastructure.SeedWork;
using System.Reflection;

/*建立 WebApplicationBuilder 物件*/
var builder = WebApplication.CreateBuilder(args);

/*透過 builder.Services 將服務加入 DI 容器*/

/*註冊控制器的服務*/
builder.Services.AddControllers(opt =>
{
    var oldValueProviderFactory = opt.ValueProviderFactories.FirstOrDefault(x => x is IValueProviderFactory);
    if (oldValueProviderFactory != null)
    {
        opt.ValueProviderFactories.Remove(oldValueProviderFactory); //移除預設的JSON序列化程式
    }
    opt.ValueProviderFactories.Insert(0, new SnakeCaseQueryValueProviderFactory()); //載入SnakeCase形式JSON序列化程式
}).ConfigureApiBehaviorOptions(opt =>
{
    /*系統預設是透過 ModelStateInvalidFilter 來篩出 Model 驗證失敗的 Request 作錯誤回應，
     * 將該錯誤資訊收集後以 HTTP Status 400 連同錯誤資訊回應給呼叫端；因此我們可以透過 
     * SuppressModelStateInvalidFilter = true 設定來停止這個 Filter 發生作用，
     * 成功設定後當 Model 驗證失敗則不會有錯誤回應*/
    opt.SuppressModelStateInvalidFilter = true;
});

/*是ASP.NET Core自身提供給 MinimalApi */
//builder.Services.AddEndpointsApiExplorer();

/*添加swagger配置*/
builder.Services.AddSwaggerGen(c =>
{
    /*套用Filter*/
    c.OperationFilter<SnakecasingParameOperationFilter>(); //QueryString 參數選擇使用SnakeCase 的形式
    c.SwaggerDoc("v1", new OpenApiInfo { Title = Assembly.GetExecutingAssembly().GetName().Name, Version = "v1" });
    /*在 Swagger UI 看見我們的 Authorize 按鈕*/
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT"
    });
    /*全部的呼叫都自動加上這個Bearer開頭的 Token*/
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
    /*開啟支持在屬性上添加註釋*/
    c.EnableAnnotations(); //Swashbuckle.AspNetCore.Annotations 套件
});

/*註冊 MediatR*/
builder.Services.AddMediatR(typeof(BaseCommand).GetTypeInfo().Assembly);

/*註冊 AutoMapper的 profile*/
var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
var profileAssemblies = assemblyNames.Select(assenbly => Assembly.Load(assenbly)).ToList();
profileAssemblies.Add(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(profileAssemblies);

// Add services to the container.
IConfiguration configuration = builder.Configuration;

/*Transient*/
/*註冊 EF DBContext */
builder.Services.AddTransient<DBContext>();
/*註冊 MediatR Pipeline 功能 : 針對MediatR的Log紀錄*/
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
/*註冊 MediatR RequestException 功能 : 針對MediatR的異常處理*/
builder.Services.AddTransient(typeof(IRequestExceptionHandler<,>), typeof(ExceptionBehavior<,>));

/*註冊 EF mySql Connection*/
builder.Services.AddDbContext<DBContext>(options =>
    options.UseMySql(configuration.GetSection("MYSqlConnection:Main").Value, ServerVersion.AutoDetect(configuration.GetSection("MYSqlConnection:Main").Value))
);

/* 類別注入使用IServiceCollection進行註冊，共有以下三種生命週期:
 * Transient：每次注入時都回傳新的物件。
 * Scoped：   在同一個Request中注入會回傳同一個物件。
 * Singleton：僅於第一次注入時建立新物件，後面注入時會拿到第一次建立的物件(只要執行緒還活著)。*/

/*Scoped*/
/*註冊 Dapper Connection*/
builder.Services.AddScoped<IUnitOfWorkDapper>((context) =>
{
    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    return new UnitOfWork(new MySqlConnection(configuration.GetSection("MYSqlConnection:Main").Value), new MySqlConnection(configuration.GetSection("MYSqlConnection:Secondary").Value));
});

/*註冊 IMessageRepository*/
builder.Services.AddScoped<IMessageRepository, MessageRepository>();



/*註冊 Health Checks 服務 健康檢查的應用*/
builder.Services.AddHealthChecks();

/*建立 WebApplication 物件*/
var app = builder.Build();

/*透過 app 設定 Middlewares (HTTP request pipeline)*/
if (app.Environment.IsDevelopment())
{
    /*當環境為 Development 啟用Swagger*/
    app.UseSwagger();
    /* url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
       description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。*/
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PXGo.Study.API v1"));
}

/*執行應用程式，透過瀏覽器開啟 /health_check 就可以看到回傳健康檢查的結果*/
app.UseHealthChecks("/health_check");

/*檢查當前項目是否是訪問多個端口後，如果有協議是 Https，我們在 Http 的約定啟動轉發到 Https 中
  launchSettings.json文件中存在，“applicationUrl”*/
//app.UseHttpsRedirection(); 

/*會將路由對應新增到Middleware中。此Middleware會查看應用程式中定義的端點集合，並根據要求選取最符合的條件。
  套用 Attribute在 Controller 類別 [Route("api/[controller]")]
  套用 Attribute 在 Action 方法 [HttpGet("{id}")]*/
app.UseRouting();

/*啟用 cookie 原則功能*/
//app.UseCookiePolicy();

/*啟用身分識別*/
//app.UseAuthentication();

/*啟用授權功能*/
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    /*url約定路由(conventional routing), 通常用在MVC專案中*/
    //endpoints.MapControllerRoute(
    //    name: "default",
    //    pattern: "{controller=Home}/{action=index}/{id?}")
    //;

    /*Microsoft.AspNetCore.Routing.IEndpointRouteBuilder and adds
      the default route {controller=Home}/{action=Index}/{id?}.*/
    endpoints.MapDefaultControllerRoute();
    /*不對約定路由做任何假設，也就是不使用約定路由，依賴使用者的特性路由， 一般用在WebAPI專案中。*/
    endpoints.MapControllers();
});

// 啟動 ASP.NET Core 應用程式
app.Run();
