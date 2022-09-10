using System.Reactive.Disposables;
using Strem.Core.Variables;
using Strem.Flows.Data;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Executors;

public interface ITriggerExecutor
{
    Task SetupTrigger(Flow flow, IFlowTriggerData triggerData, CompositeDisposable flowSubs, Action<IVariables> onStart);
}