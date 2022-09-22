using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Strem.Flows.Components;

public abstract class CustomFlowElementComponent : ComponentBase
{
    public abstract string Title { get; }
    private string _previousTitle;
    
    [Parameter]
    public EventCallback<string> OnTitleChanged { get; set; }
    
    [CascadingParameter]
    public EditContext? EditContext { get; set; }

    protected override Task OnInitializedAsync()
    {
        _previousTitle = Title;
        OnTitleChanged.InvokeAsync(Title);
        return base.OnInitializedAsync();
    }

    protected override Task OnParametersSetAsync()
    {
        if (Title != _previousTitle)
        {
            _previousTitle = Title;
            OnTitleChanged.InvokeAsync(Title);
        }
        return base.OnParametersSetAsync();
    }
}