using Newtonsoft.Json;
using Strem.Core.Extensions;
using Strem.Portals.Services.Stores;

namespace Strem.Portals.Services.Data;

public class PortalExporter : IPortalExporter
{
    public ILogger<PortalExporter> Logger { get; }
    public IPortalStore PortalStore { get; }

    public PortalExporter(ILogger<PortalExporter> logger, IPortalStore portalStore)
    {
        Logger = logger;
        PortalStore = portalStore;
    }

    public string Export(IEnumerable<Guid> ids)
    {
        var elementsToExport = ids
            .Select(PortalStore.Get)
            .ToArray();

        if (elementsToExport.Length == 0)
        {
            Logger.Warning("Cant find any portals to export");
            return string.Empty;
        }
        
        var flowContainer = new PortalDataWrapper(elementsToExport);

        try
        { return JsonConvert.SerializeObject(flowContainer); }
        catch (Exception e)
        {
            Logger.Error($"Couldnt export portals, error: {e.Message}");
            return string.Empty;
        }
    }
}