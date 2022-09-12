using Newtonsoft.Json;
using Strem.Core.Extensions;
using Strem.Flows.Services.Stores;

namespace Strem.Flows.Services.Data;

public class FlowExporter : IFlowExporter
{
    public ILogger<FlowExporter> Logger { get; }
    public IFlowStore FlowStore { get; }

    public FlowExporter(ILogger<FlowExporter> logger, IFlowStore flowStore)
    {
        Logger = logger;
        FlowStore = flowStore;
    }

    public string Export(IEnumerable<Guid> ids)
    {
        var flowsToExport = ids
            .Select(FlowStore.Get)
            .ToArray();

        if (flowsToExport.Length == 0)
        {
            Logger.Warning("Cant find any flows to export");
            return string.Empty;
        }
        
        var flowContainer = new FlowDataWrapper(flowsToExport);

        try
        { return JsonConvert.SerializeObject(flowContainer); }
        catch (Exception e)
        {
            Logger.Error($"Couldnt export flows, error: {e.Message}");
            return string.Empty;
        }
    }
}