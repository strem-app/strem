using System.Reactive.Disposables;
using Strem.Core.Extensions;
using Strem.Core.Flows.Registries.Triggers;
using Strem.Core.Flows.Triggers;
using Strem.Core.Validation;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public class TriggerExecutor : ITriggerExecutor
{
    public ITriggerRegistry TriggerRegistry { get; }
    public ILogger<TriggerExecutor> Logger { get; }
    public IDataValidator DataValidator { get; }

    public TriggerExecutor(ITriggerRegistry triggerRegistry, ILogger<TriggerExecutor> logger, IDataValidator dataValidator)
    {
        TriggerRegistry = triggerRegistry;
        Logger = logger;
        DataValidator = dataValidator;
    }

    public async Task SetupTrigger(Flow flow, IFlowTriggerData triggerData, CompositeDisposable flowSubs, Action<IVariables> onStart)
    {
        var trigger = TriggerRegistry.Get(triggerData.Code)?.Trigger;
        if (trigger == null)
        {
            Logger.LogWarning($"Trigger cant be found for trigger code: {triggerData.Code} (v{triggerData.Version})");
            return;
        }

        var validationResults = DataValidator.Validate(triggerData);
        if (!validationResults.IsValid)
        {
            Logger.LogWarning($"Trigger data contains invalid data for {triggerData.Id}|{triggerData.Code}, with errors {string.Join(" | ", validationResults.Errors)}");
            return;
        }
            
        var observable = await trigger.Execute(triggerData);
        observable.Subscribe(onStart).AddTo(flowSubs);
    }
}