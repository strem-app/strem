using Microsoft.Extensions.Logging;
using Moq;

namespace Strem.UnitTests.Extensions;

public static class MockILoggerExtensions
{
    public static void VerifyLogsFor<T>(this Mock<ILogger<T>> logger, LogLevel logType, string? message = null, Exception? exception = null, Times? times = null)
    {
        logger.Verify(x => x.Log(logType, 
            It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) =>  message == null || string.Equals(message, o.ToString(), StringComparison.InvariantCultureIgnoreCase)), 
            It.Is<Exception>(x => exception == null || x == exception), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times ?? Times.Once());
    }
}