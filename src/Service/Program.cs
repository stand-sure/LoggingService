using System.Text.RegularExpressions;

using Serilog;
using Serilog.Extensions.Logging;

using Service;

const string healthCheckPath = "/healthz";
const string requestWarningPatternConfigKey = "requestWarningPattern";

AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(2));
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(ConfigureLogger);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app.UseHealthChecks(healthCheckPath);
app.UseWhen(IsNotHealthCheck, UseMyMiddleware(builder.Configuration));

app.Run();

// ReSharper disable once SeparateLocalFunctionsWithJumpStatement
static void ConfigureLogger(HostBuilderContext context, IServiceProvider serviceProvider, LoggerConfiguration loggerConfiguration)
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(serviceProvider);
}

static Action<IApplicationBuilder> UseMyMiddleware(IConfiguration configuration)
{
    string warningPattern = configuration[requestWarningPatternConfigKey] ?? "";
    
    return applicationBuilder => applicationBuilder.Use(MyMiddlewareFactory(warningPattern));
}

static bool IsNotHealthCheck(HttpContext context)
{
    return context.Request.Path.Value?.StartsWith(healthCheckPath) != true;
}

static Func<HttpContext, RequestDelegate, Task> MyMiddlewareFactory(string warningPattern)
{
    var regex = new Regex(warningPattern);
    
    return MyMiddleware;

    Task MyMiddleware(HttpContext context, RequestDelegate requestDelegate)
    {
        ILogger<Program> logger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();

        regex.IsMatch(context.Request.Headers["X-Original-URI"].ToString()).Match(Warn, Info);

        return Task.CompletedTask;

        void Warn()
        {
            logger.RequestWarn();
        }

        void Info()
        {
            logger.RequestInfo();
        }
    }
}