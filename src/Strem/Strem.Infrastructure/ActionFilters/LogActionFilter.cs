using Microsoft.AspNetCore.Mvc.Filters;
using Strem.Infrastructure.Extensions;

namespace Strem.Infrastructure.ActionFilters;

public class LogActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    { WebHostHackExtensions.Logger.Information($"[{context.ActionDescriptor.DisplayName}] Executing"); }

    public void OnActionExecuted(ActionExecutedContext context)
    { WebHostHackExtensions.Logger.Information($"[{context.ActionDescriptor.DisplayName}] Executed -> {context.HttpContext.Response.StatusCode}"); }
}