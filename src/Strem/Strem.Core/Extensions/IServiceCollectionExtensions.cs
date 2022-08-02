using Microsoft.Extensions.DependencyInjection;
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
        
    public static IServiceCollection AddModule<T>(this IServiceCollection services) where T : IDependencyModule, new()
    { return services.AddModule(new T()); }
    
    public static IServiceCollection AddTaskDescriptor<TData, TComponent>(this IServiceCollection serviceCollection)
        where TData : IFlowTaskData, new()
    {
        return serviceCollection.AddSingleton(x => 
            new TaskDescriptor(x.GetService<IFlowTask<TData>>(),
            () => new TData(), typeof(TComponent)));
    }
    
    public static IServiceCollection AddTriggerDescriptor<TData, TComponent>(this IServiceCollection serviceCollection)
        where TData : IFlowTriggerData, new()
    {
        return serviceCollection.AddSingleton(x => 
            new TriggerDescriptor(x.GetService<IFlowTrigger<TData>>(),
            () => new TData(), typeof(TComponent)));
    }
}