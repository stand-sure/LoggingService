namespace Service;

internal static partial class LogMessages
{
    [LoggerMessage(LogLevel.Information, "")]
    internal static partial void RequestInfo(this ILogger<Program> logger);
    
    [LoggerMessage(LogLevel.Warning, "")]
    internal static partial void RequestWarn(this ILogger<Program> logger);
}