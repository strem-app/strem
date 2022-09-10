using System.Reflection;
using Strem.Core.Extensions;
using Strem.Flows.Components.Tasks;
using Strem.Flows.Components.Triggers;
using Strem.Flows.Data.Tasks;
using Strem.Flows.Data.Triggers;
using Strem.Flows.Services.Registries.Tasks;
using Strem.Flows.Services.Registries.Triggers;

namespace Strem.Flows.Extensions;

public static class IServiceCollectionComponentExtensions
{
    public static readonly Type FlowTaskInterfaceType = typeof(IFlowTask);
    public static readonly Type FlowTaskType = typeof(FlowTask<>);
    public static readonly Type FlowTaskComponentType = typeof(CustomTaskComponent<>);
    public static readonly Type FlowTriggerInterfaceType = typeof(IFlowTrigger);
    public static readonly Type FlowTriggerType = typeof(FlowTrigger<>);
    public static readonly Type FlowTriggerComponentType = typeof(CustomTriggerComponent<>);
    
    public static IServiceCollection AddTaskDescriptor<TData, TComponent>(this IServiceCollection serviceCollection)
        where TData : IFlowTaskData, new()
    {
        return serviceCollection.AddSingleton(x => 
            new TaskDescriptor(x.GetService<FlowTask<TData>>(),
                () => new TData(), typeof(TComponent)));
    }
    
    public static IServiceCollection AddTaskDescriptor(this IServiceCollection serviceCollection, Type flowTaskDataType, Type componentType)
    {
        var flowTaskGenericType = FlowTaskType.MakeGenericType(flowTaskDataType);
        return serviceCollection.AddSingleton(x => 
            new TaskDescriptor(x.GetService(flowTaskGenericType) as IFlowTask,
                () => Activator.CreateInstance(flowTaskDataType) as IFlowTaskData, 
                componentType));
    }
    
    public static IServiceCollection AddTriggerDescriptor<TData, TComponent>(this IServiceCollection serviceCollection)
        where TData : IFlowTriggerData, new()
    {
        return serviceCollection.AddSingleton(x => 
            new TriggerDescriptor(x.GetService<FlowTrigger<TData>>(),
                () => new TData(), typeof(TComponent)));
    }
    
    public static IServiceCollection AddTriggerDescriptor(this IServiceCollection serviceCollection, Type flowTriggerDataType, Type componentType)
    {
        var flowTriggerGenericType = FlowTriggerType.MakeGenericType(flowTriggerDataType);
        return serviceCollection.AddSingleton(x => 
            new TriggerDescriptor(x.GetService(flowTriggerGenericType) as IFlowTrigger,
                () => Activator.CreateInstance(flowTriggerDataType) as IFlowTriggerData,
                componentType));
    }
    
    public static void RegisterAllTasksAndComponentsIn(this IServiceCollection services, Assembly assembly)
    {
        var allTypes = assembly.GetTypes();
        var customTaskTypes = allTypes.WhereClassesImplement(FlowTaskInterfaceType);

        foreach (var customTaskType in customTaskTypes)
        {
            var genericDataTypes = customTaskType.BaseType.GetGenericArguments();
            if(genericDataTypes.Length == 0) { continue; }
            var genericDataType = genericDataTypes[0];
            services.AddSingleton(customTaskType.BaseType, customTaskType);

            var genericFlowTaskComponentType = FlowTaskComponentType.MakeGenericType(genericDataType);
            var componentTypesMatching = allTypes.WhereClassesInheritFrom(genericFlowTaskComponentType);
            if(componentTypesMatching.Length == 0) { continue; }
            services.AddTaskDescriptor(genericDataType, componentTypesMatching[0]);
        }
    }
    
    public static void RegisterAllTriggersAndComponentsIn(this IServiceCollection services, Assembly assembly)
    {
        var allTypes = assembly.GetTypes();
        var customTriggerTypes = allTypes.WhereClassesImplement(FlowTriggerInterfaceType);

        foreach (var customTriggerType in customTriggerTypes)
        {
            var genericDataTypes = customTriggerType.BaseType.GetGenericArguments();
            if(genericDataTypes.Length == 0) { continue; }
            var genericDataType = genericDataTypes[0];
            services.AddSingleton(customTriggerType.BaseType, customTriggerType);

            var genericFlowTriggerComponentType = FlowTriggerComponentType.MakeGenericType(genericDataType);
            var componentTypesMatching = allTypes.WhereClassesInheritFrom(genericFlowTriggerComponentType);
            if(componentTypesMatching.Length == 0) { continue; }
            services.AddTriggerDescriptor(genericDataType, componentTypesMatching[0]);
        }
    }
}