using System.Reactive;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Triggers;

public interface IFlowTrigger<in T> where T : IFlowTriggerData
{
    string TriggerVersion { get; }
    string TriggerCode { get; }
    string Name { get; }
    string Description { get; }
    
    IObservable<Unit> Execute(T data, IVariables flowVars);
}