using System.Reactive.Disposables;
using System.Reactive.Linq;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Plugins;
using Strem.Flows.Events;
using Strem.Flows.Events.Base;
using Strem.Flows.Events.Tasks;
using Strem.Flows.Events.Triggers;
using Strem.Flows.Executors;
using Strem.Flows.Services.Registries.Tasks;
using Strem.Flows.Services.Registries.Triggers;
using Strem.Flows.Services.Repositories;
using Strem.Flows.Services.Stores;

namespace Strem.Flows.Plugins;

public class FlowsPluginStartup : IPluginStartup, IDisposable
{
    private readonly CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IFlowStore FlowStore { get; }
    public IFlowRepository FlowRepository { get; }
    public ILogger<FlowsPluginStartup> Logger { get; }
    public ITaskRegistry TaskRegistry { get; }
    public ITriggerRegistry TriggerRegistry { get; }
    public IFlowExecutionEngine ExecutionEngine { get; }
    public IServiceProvider Services { get; }

    public string[] RequiredConfigurationKeys { get; } = Array.Empty<string>();

    public FlowsPluginStartup(IEventBus eventBus, IFlowStore flowStore, IFlowRepository flowRepository, ILogger<FlowsPluginStartup> logger, ITaskRegistry taskRegistry, ITriggerRegistry triggerRegistry, IFlowExecutionEngine executionEngine, IServiceProvider services)
    {
        EventBus = eventBus;
        FlowStore = flowStore;
        FlowRepository = flowRepository;
        Logger = logger;
        TaskRegistry = taskRegistry;
        TriggerRegistry = triggerRegistry;
        ExecutionEngine = executionEngine;
        Services = services;
    }

    public Task SetupPlugin() => Task.CompletedTask;

    public async Task StartPlugin()
    {
        EventBus
            .ReceiveAs<FlowEvent, FlowTaskChangedEvent, FlowTriggerChangedEvent, FlowDetailsChangedEvent, FlowEnabledEvent, FlowDisabledEvent>()
            .ThrottledByKey(x => x, TimeSpan.FromSeconds(2))
            .Select(x => FlowStore.Get(x.FlowId))
            .Subscribe(x =>
            {
                if(x == null){ return; }
                FlowRepository.Update(x.Id, x);
            })
            .AddTo(_subs);

        EventBus.Receive<ApplicationPluginsLoadedEvent>()
            .Subscribe(x => StartEventExecutor())
            .AddTo(_subs);

        Logger.Information("Setting Up Flow Registries");
        StartRegistries();
        Logger.Information("Finished Setting Up Flow Registries");
    }

    public async Task StartEventExecutor()
    {
        Logger.Information("Starting Flow Execution Engine");
        await ExecutionEngine.StartEngine();
        Logger.Information("Started Flow Execution Engine");
    }

    public void StartRegistries()
    {
        try
        {
            var taskDescriptors = Services.GetServices<TaskDescriptor>();
            TaskRegistry?.AddMany(taskDescriptors);
            
            var triggerDescriptors = Services.GetServices<TriggerDescriptor>();
            TriggerRegistry?.AddMany(triggerDescriptors);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
            throw;
        }
    }
    
    public void Dispose()
    {
        _subs.Dispose();
    }
}