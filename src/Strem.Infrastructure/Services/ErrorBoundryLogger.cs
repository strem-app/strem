using Microsoft.AspNetCore.Components.Web;
using Strem.Core.Extensions;

namespace Strem.Infrastructure.Services;

public class ErrorBoundryLogger : IErrorBoundaryLogger
{
    public ILogger<IErrorBoundaryLogger> Logger { get; }

    public ErrorBoundryLogger(ILogger<IErrorBoundaryLogger> logger)
    {
        Logger = logger;
    }

    public async ValueTask LogErrorAsync(Exception exception)
    {
        Logger.Error(exception);
    }
}