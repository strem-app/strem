using Microsoft.AspNetCore.Mvc;
using Strem.Core.Events;
using Strem.Core.State;
using ILogger = Serilog.ILogger;

namespace Strem.Infrastructure.Extensions;

// TODO: This will be removed once shared service collection stuff can be sorted
public static class WebHostHackExtensions
{
    public static IEventBus EventBus { get; set; }
    public static ILogger Logger { get; set; }
    public static IAppState AppState { get; set; }
    public static IServiceProvider ServiceLocator { get; set; }

    public static void PublishEvent<T>(this ControllerBase controller, T eventArgs) => EventBus.Publish(eventArgs);
    public static void PublishAsyncEvent<T>(this ControllerBase controller, T eventArgs) => EventBus.PublishAsync(eventArgs);
    public static IAppState GetAppState(this ControllerBase controller) => AppState;
}