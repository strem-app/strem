using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Strem.Flows.Data.Tasks;

namespace Strem.Flows.Components.Tasks;

public abstract class CustomTaskComponent<T> : CustomFlowElementComponent
    where T : class, IFlowTaskData
{
    [Parameter]
    public T Data { get; set; }
    
    public void NotifyPropertyChanged(string propertyName)
    {
        EditContext?.NotifyFieldChanged(new FieldIdentifier(Data, propertyName));
    }
}