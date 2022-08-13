using Microsoft.AspNetCore.Components;

namespace Strem.Core.Components;

public abstract class CustomFlowElementComponent : ComponentBase
{
    public abstract string Title { get; }
    private string _previousTitle;
    
    [Parameter]
    public EventCallback<string> OnTitleChanged { get; set; }

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