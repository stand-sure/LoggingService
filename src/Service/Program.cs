using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, serviceProvider, loggingConfig) => loggingConfig
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(serviceProvider));

builder.Services.AddLogging();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();

app.Use(Middleware);

app.Run();

// ReSharper disable once SeparateLocalFunctionsWithJumpStatement
static Task Middleware(HttpContext context, RequestDelegate requestDelegate)
{
    return Task.CompletedTask;
}