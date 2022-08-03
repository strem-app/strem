using Microsoft.Extensions.DependencyInjection;
using Strem.Core.DI;
using Strem.Core.Extensions;

namespace Strem.Flows.Default.Modules;

public class DefaultFlowsModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        var thisAssembly = GetType().Assembly;
        
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
    }
}