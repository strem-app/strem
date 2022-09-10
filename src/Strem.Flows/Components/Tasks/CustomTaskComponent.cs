using Microsoft.AspNetCore.Components;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Components.Tasks;

public abstract class CustomTaskComponent<T> : CustomFlowElementComponent
    where T : class, IFlowTaskData
{
    [Parameter]
    public T Data { get; set; }
}