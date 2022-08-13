using Microsoft.Extensions.Logging;

namespace Strem.Core.Extensions;

public static class ILoggerExtensions
{
    public static void Information(this ILogger logger, string message) => logger.Log(LogLevel.Information, message);
    public static void Warning(this ILogger logger, string message) => logger.Log(LogLevel.Warning, message);
    public static void Error(this ILogger logger, string message) => logger.Log(LogLevel.Error, message);
    public static void Error(this ILogger logger, Exception exception) => logger.Log(LogLevel.Error, exception, exception.Message);
}