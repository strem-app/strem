using System.Reactive;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Triggers;

public interface IFlowTrigger : IFlowElement
{
    bool CanExecute();
    Task<IObservable<IVariables>> Execute(object data);
}