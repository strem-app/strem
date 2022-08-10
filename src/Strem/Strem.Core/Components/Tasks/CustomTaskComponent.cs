using Microsoft.AspNetCore.Components;
using Strem.Core.Flows.Tasks;

namespace Strem.Core.Components;

public abstract class CustomTaskComponent<T> : CustomFlowElementComponent
    where T : class, IFlowTaskData
{
    [Parameter]
    public T Data { get; set; }
}