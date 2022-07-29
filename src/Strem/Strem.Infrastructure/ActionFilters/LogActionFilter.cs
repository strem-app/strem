using Microsoft.AspNetCore.Mvc.Filters;
using Strem.Infrastructure.Extensions;
using ILogger = Serilog.ILogger;

namespace Strem.Infrastructure.ActionFilters;

public class LogActionFilter : IActionFilter
{
    public ILogger Logger { get; }
    
    public LogActionFilter()
    {
        Logger = WebHostHackExtensions.ServiceLocator.GetService<ILogger>();
    }

    public void OnActionExecuting(ActionExecutingContext context)
    { Logger.Information($"[{context.ActionDescriptor.DisplayName}] Executing"); }

    public void OnActionExecuted(ActionExecutedContext context)
    { Logger.Information($"[{context.ActionDescriptor.DisplayName}] Executed -> {context.HttpContext.Response.StatusCode}"); }
}