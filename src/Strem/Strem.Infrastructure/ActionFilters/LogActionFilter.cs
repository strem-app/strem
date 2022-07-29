using Microsoft.AspNetCore.Mvc.Filters;
using Strem.Core.Extensions;
using Strem.Infrastructure.Extensions;

namespace Strem.Infrastructure.ActionFilters;

public class LogActionFilter : IActionFilter
{
    public ILogger<LogActionFilter> Logger { get; }
    
    public LogActionFilter()
    {
        Logger = WebHostHackExtensions.ServiceLocator.GetService<ILogger<LogActionFilter>>();
    }

    public void OnActionExecuting(ActionExecutingContext context)
    { Logger.Information($"[{context.ActionDescriptor.DisplayName}] Executing"); }

    public void OnActionExecuted(ActionExecutedContext context)
    { Logger.Information($"[{context.ActionDescriptor.DisplayName}] Executed -> {context.HttpContext.Response.StatusCode}"); }
}