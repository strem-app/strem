using Newtonsoft.Json;
using Strem.Core.Extensions;
using Strem.Flows.Services.Stores;

namespace Strem.Flows.Services.Data;

public class FlowImporter : IFlowImporter
{
    public ILogger<FlowImporter> Logger { get; }
    public IFlowStore FlowStore { get; }

    public FlowImporter(ILogger<FlowImporter> logger, IFlowStore flowStore)
    {
        Logger = logger;
        FlowStore = flowStore;
    }

    public int Import(string jsonContent)
    {
        if (string.IsNullOrEmpty(jsonContent))
        {
            Logger.Error($"Could not import flows, error: There is no JSON data provided to read from");
            return 0;
        }
        
        FlowDataWrapper dataWrapper;
        try
        { dataWrapper = JsonConvert.DeserializeObject<FlowDataWrapper>(jsonContent); }
        catch (Exception ex)
        {
            Logger.Error($"Could not import flows, error: {ex.Message}");
            return 0;
        }

        var flowsImported = 0;
        foreach (var flow in dataWrapper.Flows)
        {
            if (FlowStore.Has(flow.Id))
            {
                Logger.Warning($"Flow already exists with same id for {flow.Name}, ignoring on import process");
                continue;
            }

            FlowStore.Add(flow);
            flowsImported++;
            Logger.Information($"Imported Flow - {flow.Name}");
        }
        return flowsImported;
    }
}