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

/*�إ� WebApplicationBuilder ����*/
var builder = WebApplication.CreateBuilder(args);

/*�z�L builder.Services �N�A�ȥ[�J DI �e��*/

/*���U������A��*/
builder.Services.AddControllers(opt =>
{
    var oldValueProviderFactory = opt.ValueProviderFactories.FirstOrDefault(x => x is IValueProviderFactory);
    if (oldValueProviderFactory != null)
    {
        opt.ValueProviderFactories.Remove(oldValueProviderFactory); //�����w�]��JSON�ǦC�Ƶ{��
    }
    opt.ValueProviderFactories.Insert(0, new SnakeCaseQueryValueProviderFactory()); //���JSnakeCase�Φ�JSON�ǦC�Ƶ{��
}).ConfigureApiBehaviorOptions(opt =>
{
    /*�t�ιw�]�O�z�L ModelStateInvalidFilter �ӿz�X Model ���ҥ��Ѫ� Request �@���~�^���A
     * �N�ӿ��~��T������H HTTP Status 400 �s�P���~��T�^�����I�s�ݡF�]���ڭ̥i�H�z�L 
     * SuppressModelStateInvalidFilter = true �]�w�Ӱ���o�� Filter �o�ͧ@�ΡA
     * ���\�]�w��� Model ���ҥ��ѫh���|�����~�^��*/
    opt.SuppressModelStateInvalidFilter = true;
});

/*�OASP.NET Core�ۨ����ѵ� MinimalApi */
//builder.Services.AddEndpointsApiExplorer();

/*�K�[swagger�t�m*/
builder.Services.AddSwaggerGen(c =>
{
    /*�M��Filter*/
    c.OperationFilter<SnakecasingParameOperationFilter>(); //QueryString �Ѽƿ�ܨϥ�SnakeCase ���Φ�
    c.SwaggerDoc("v1", new OpenApiInfo { Title = Assembly.GetExecutingAssembly().GetName().Name, Version = "v1" });
    /*�b Swagger UI �ݨ��ڭ̪� Authorize ���s*/
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT"
    });
    /*�������I�s���۰ʥ[�W�o��Bearer�}�Y�� Token*/
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
    /*�}�Ҥ���b�ݩʤW�K�[����*/
    c.EnableAnnotations(); //Swashbuckle.AspNetCore.Annotations �M��
});

/*���U MediatR*/
builder.Services.AddMediatR(typeof(BaseCommand).GetTypeInfo().Assembly);

/*���U AutoMapper�� profile*/
var assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
var profileAssemblies = assemblyNames.Select(assenbly => Assembly.Load(assenbly)).ToList();
profileAssemblies.Add(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(profileAssemblies);

// Add services to the container.
IConfiguration configuration = builder.Configuration;

/*Transient*/
/*���U EF DBContext */
builder.Services.AddTransient<DBContext>();
/*���U MediatR Pipeline �\�� : �w��MediatR��Log����*/
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
/*���U MediatR RequestException �\�� : �w��MediatR�����`�B�z*/
builder.Services.AddTransient(typeof(IRequestExceptionHandler<,>), typeof(ExceptionBehavior<,>));

/*���U EF mySql Connection*/
builder.Services.AddDbContext<DBContext>(options =>
    options.UseMySql(configuration.GetSection("MYSqlConnection:Main").Value, ServerVersion.AutoDetect(configuration.GetSection("MYSqlConnection:Main").Value))
);

/* ���O�`�J�ϥ�IServiceCollection�i����U�A�@���H�U�T�إͩR�g��:
 * Transient�G�C���`�J�ɳ��^�Ƿs������C
 * Scoped�G   �b�P�@��Request���`�J�|�^�ǦP�@�Ӫ���C
 * Singleton�G�ȩ�Ĥ@���`�J�ɫإ߷s����A�᭱�`�J�ɷ|����Ĥ@���إߪ�����(�u�n������٬���)�C*/

/*Scoped*/
/*���U Dapper Connection*/
builder.Services.AddScoped<IUnitOfWorkDapper>((context) =>
{
    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    return new UnitOfWork(new MySqlConnection(configuration.GetSection("MYSqlConnection:Main").Value), new MySqlConnection(configuration.GetSection("MYSqlConnection:Secondary").Value));
});

/*���U IMessageRepository*/
builder.Services.AddScoped<IMessageRepository, MessageRepository>();



/*���U Health Checks �A�� ���d�ˬd������*/
builder.Services.AddHealthChecks();

/*�إ� WebApplication ����*/
var app = builder.Build();

/*�z�L app �]�w Middlewares (HTTP request pipeline)*/
if (app.Environment.IsDevelopment())
{
    /*�����Ҭ� Development �ҥ�Swagger*/
    app.UseSwagger();
    /* url: �ݰt�X SwaggerDoc �� name�C "/swagger/{SwaggerDoc name}/swagger.json"
       description: �Ω� Swagger UI �k�W����ܤ��P������ SwaggerDocument ��ܦW�٨ϥΡC*/
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PXGo.Study.API v1"));
}

/*�������ε{���A�z�L�s�����}�� /health_check �N�i�H�ݨ�^�ǰ��d�ˬd�����G*/
app.UseHealthChecks("/health_check");

/*�ˬd��e���جO�_�O�X�ݦh�Ӻݤf��A�p�G����ĳ�O Https�A�ڭ̦b Http �����w�Ұ���o�� Https ��
  launchSettings.json��󤤦s�b�A��applicationUrl��*/
//app.UseHttpsRedirection(); 

/*�|�N���ѹ����s�W��Middleware���C��Middleware�|�d�����ε{�����w�q�����I���X�A�îھڭn�D����̲ŦX������C
  �M�� Attribute�b Controller ���O [Route("api/[controller]")]
  �M�� Attribute �b Action ��k [HttpGet("{id}")]*/
app.UseRouting();

/*�ҥ� cookie ��h�\��*/
//app.UseCookiePolicy();

/*�ҥΨ����ѧO*/
//app.UseAuthentication();

/*�ҥα��v�\��*/
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    /*url���w����(conventional routing), �q�`�ΦbMVC�M�פ�*/
    //endpoints.MapControllerRoute(
    //    name: "default",
    //    pattern: "{controller=Home}/{action=index}/{id?}")
    //;

    /*Microsoft.AspNetCore.Routing.IEndpointRouteBuilder and adds
      the default route {controller=Home}/{action=Index}/{id?}.*/
    endpoints.MapDefaultControllerRoute();
    /*������w���Ѱ����󰲳]�A�]�N�O���ϥά��w���ѡA�̿�ϥΪ̪��S�ʸ��ѡA �@��ΦbWebAPI�M�פ��C*/
    endpoints.MapControllers();
});

// �Ұ� ASP.NET Core ���ε{��
app.Run();
