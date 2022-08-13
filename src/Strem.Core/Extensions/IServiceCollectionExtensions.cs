using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Strem.Core.Components.Tasks;
using Strem.Core.DI;
using Strem.Core.Flows.Registries;
using Strem.Core.Flows.Tasks;
using Strem.Core.Flows.Triggers;

namespace Strem.Core.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddModule(this IServiceCollection services, IDependencyModule dependencyModule)
    {
        dependencyModule.Setup(services);
        return services;
    }
    
    public static IServiceCollection AddModules(this IServiceCollection services, params IDependencyModule[] dependencyModules)
    {
        foreach(var dependencyModule in dependencyModules)
        { dependencyModule.Setup(services); }
        return services;
    }    
    
    public static IServiceCollection AddModules(this IServiceCollection services, params Type[] dependencyModuleTypes)
    {
        foreach (var dependencyModuleType in dependencyModuleTypes)
        {
            var dependencyModule = (IDependencyModule)Activator.CreateInstance(dependencyModuleType);
            dependencyModule.Setup(services);
        }
        return services;
    }
        
    public static IServiceCollection AddModule<T>(this IServiceCollection services) where T : IDependencyModule, new()
    { return services.AddModule(new T()); }
}