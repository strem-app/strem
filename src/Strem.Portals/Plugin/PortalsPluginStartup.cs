using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Portals.Data;
using Strem.Portals.Services.Persistence;

namespace Strem.Portals.Plugin;

public class PortalsPluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public ISavePortalStorePipeline SavePipeline { get; }
    public IPortalStore PortalStore { get; }
    public ILogger<PortalsPluginStartup> Logger { get; }

    public PortalsPluginStartup(ISavePortalStorePipeline savePipeline, IPortalStore portalStore, ILogger<PortalsPluginStartup> logger)
    {
        SavePipeline = savePipeline;
        PortalStore = portalStore;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
        Observable.Interval(TimeSpan.FromMinutes(5))
            .Subscribe(x =>
            {
                Logger.Information("Auto Saving Portals");
                SavePipeline.Execute(PortalStore);
            })
            .AddTo(_subs);
    }

    public void Dispose()
    {
        _subs.Dispose();
    }
}