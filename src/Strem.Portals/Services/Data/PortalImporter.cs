using Newtonsoft.Json;
using Strem.Core.Extensions;
using Strem.Portals.Services.Stores;

namespace Strem.Portals.Services.Data;

public class PortalImporter : IPortalImporter
{
    public ILogger<PortalImporter> Logger { get; }
    public IPortalStore PortalStore { get; }

    public PortalImporter(ILogger<PortalImporter> logger, IPortalStore portalStore)
    {
        Logger = logger;
        PortalStore = portalStore;
    }

    public int Import(string jsonContent)
    {
        if (string.IsNullOrEmpty(jsonContent))
        {
            Logger.Error($"Could not import portals, error: There is no JSON data provided to read from");
            return 0;
        }
        
        PortalDataWrapper dataWrapper;
        try
        { dataWrapper = JsonConvert.DeserializeObject<PortalDataWrapper>(jsonContent); }
        catch (Exception ex)
        {
            Logger.Error($"Could not import portals, error: {ex.Message}");
            return 0;
        }

        if (dataWrapper?.Portals == null || dataWrapper.Portals.Count == 0)
        {
            Logger.Warning($"Imported Portal Data is empty");
            return 0;
        }

        var numberImported = 0;
        foreach (var portal in dataWrapper.Portals)
        {
            if (PortalStore.Has(portal.Id))
            {
                Logger.Warning($"Portal already exists with same id for {portal.Name}, ignoring on import process");
                continue;
            }

            PortalStore.Add(portal);
            numberImported++;
            Logger.Information($"Imported Portal - {portal.Name}");
        }
        return numberImported;
    }
}