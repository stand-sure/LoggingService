using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Serilog;
using Serilog.Extensions.Logging;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(ConfigureLogger);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
app.UseWhen(IsHealthCheck, UseHealthCheckMiddleware);
app.UseWhen(IsNotHealthCheck, UseMyMiddleware);

app.Run();

// ReSharper disable once SeparateLocalFunctionsWithJumpStatement
static void ConfigureLogger(HostBuilderContext context, IServiceProvider serviceProvider, LoggerConfiguration loggerConfiguration)
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(serviceProvider);
}

static void UseMyMiddleware(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.Use(MyMiddleware);
}

static void UseHealthCheckMiddleware(IApplicationBuilder applicationBuilder)
{
    applicationBuilder.UseMiddleware<HealthCheckMiddleware>();
}

static bool IsNotHealthCheck(HttpContext context)
{
    return !IsHealthCheck(context);
}

static bool IsHealthCheck(HttpContext context)
{
    return context.Request.Path.Value?.StartsWith("/healthz") == true;
}

static Task MyMiddleware(HttpContext context, RequestDelegate requestDelegate)
{
    (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development).IfTrue(LogHeaders);

    return Task.CompletedTask;

    void LogHeaders()
    {
        ILogger<Program> logger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();
        IDictionary<string, string> headers = context.Request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.ToString());

        logger.Headers(headers);
    }
}

internal static class BooleanExtensions
{
    internal static void IfTrue(this bool value, Action action)
    {
        if (value)
        {
            action();
        }
    }
}

internal static partial class LogMessages
{
    [LoggerMessage(LogLevel.Debug, "Headers {Headers}")]
    internal static partial void Headers(this ILogger<Program> logger, IDictionary<string, string> headers);
}