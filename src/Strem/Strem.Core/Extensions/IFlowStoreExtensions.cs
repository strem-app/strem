using Strem.Core.Flows;

namespace Strem.Core.Extensions;

public static class IFlowStoreExtensions
{
    public static Flow Create(this IFlowStore flowStore, string name, string category = FlowStore.DefaultFlowStore)
    {
        var newFlow = new Flow(Guid.NewGuid(), name, category);
        return flowStore.Add(newFlow);
    }

    public static Flow Add(this IFlowStore flowStore, Flow flow)
    {
        flowStore.Flows.Add(flow);
        return flow;
    }

    public static Flow Get(this IFlowStore flowStore, Guid flowId)
    {
        return flowStore.Flows.SingleOrDefault(x => x.Id == flowId);;
    }
    
    public static void Remove(this IFlowStore flowStore, Flow flow)
    {
        flowStore.Flows.Remove(flow);
    }
    
    public static void Remove(this IFlowStore flowStore, Guid flowId)
    {
        var flow = flowStore.Get(flowId);
        if(flow != null) { flowStore.Remove(flow); }
    }
}