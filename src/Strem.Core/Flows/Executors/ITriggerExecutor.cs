using System.Reactive.Disposables;
using Strem.Core.Flows.Triggers;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Executors;

public interface ITriggerExecutor
{
    Task SetupTrigger(Flow flow, IFlowTriggerData triggerData, CompositeDisposable flowSubs, Action<IVariables> onStart);
}