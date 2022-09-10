using Microsoft.AspNetCore.Components;
using Strem.Flows.Components;
using Strem.Flows.Data.Triggers;

namespace Strem.Flows.Components.Triggers;

public abstract class CustomTriggerComponent<T> : CustomFlowElementComponent
    where T : class, IFlowTriggerData
{
    [Parameter]
    public T Data { get; set; }
}