using System.Reactive;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Triggers;

public interface IFlowTrigger : IFlowElement {}

public interface IFlowTrigger<in T> : IFlowTrigger 
    where T : IFlowTriggerData
{
    IObservable<Unit> Execute(T data, IVariables flowVars);
}