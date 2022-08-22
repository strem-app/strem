using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Portals.Data;
using Strem.Portals.Services.Persistence;

namespace Strem.Portals.Plugin;

public class PortalsPluginStartup : IPluginStartup
{
    private CompositeDisposable _subs = new();
    
    public ISavePortalStorePipeline SavePipeline { get; set; }
    public IPortalStore PortalStore { get; set; }

    public async Task StartPlugin()
    {
        Observable.Interval(TimeSpan.FromMinutes(5))
            .Subscribe(x =>
            {
                SavePipeline.Execute(PortalStore);
            })
            .AddTo(_subs);
    }

    public void Dispose()
    {
        _subs.Dispose();
    }
}