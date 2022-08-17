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
    
    public static void AddSingleton<I1, I2, T>(this IServiceCollection services) 
        where T : class, I1, I2
        where I1 : class
        where I2 : class
    {
        services.AddSingleton<I1, T>();
        services.AddSingleton<I2, T>(x => (T) x.GetService<I1>());
    }
    
    public static void AddSingleton<I1, I2, I3, T>(this IServiceCollection services) 
        where T : class, I1, I2, I3
        where I1 : class
        where I2 : class
        where I3 : class
    {
        services.AddSingleton<I1, T>();
        services.AddSingleton<I2, T>(x => (T) x.GetService<I1>());
        services.AddSingleton<I3, T>(x => (T) x.GetService<I1>());
    }
}