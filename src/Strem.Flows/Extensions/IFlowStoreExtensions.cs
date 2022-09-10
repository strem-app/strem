using Strem.Flows.Data;
using Strem.Flows.Services.Stores;

namespace Strem.Flows.Extensions;

public static class IFlowStoreExtensions
{
    public static Flow Create(this IFlowStore flowStore, string name, string category = "Default")
    {
        var newFlow = new Flow(Guid.NewGuid(), name, category);
        return flowStore.Add(newFlow);
    }
}