using Strem.Core.Variables;

namespace Strem.Flows.Data.Triggers;

public interface IFlowTrigger : IFlowElement
{
    bool CanExecute();
    Task<IObservable<IVariables>> Execute(object data);
}