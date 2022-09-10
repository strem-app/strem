using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Strem.Core.Components.Elements.Inputs;

public abstract class ValidatedInputBase<T> : ComponentBase
{
    [Parameter]
    public T Value { get; set; }
    
    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }
    
    [Parameter] 
    public Expression<Func<T>>? ValueExpression { get; set; }

    [Parameter(CaptureUnmatchedValues = true)] 
    public IReadOnlyDictionary<string, object> UnmatchedAttributes { get; set; }
    
    [CascadingParameter] 
    protected EditContext? EditContext { get; set; } = null;
    
    protected FieldIdentifier? _fieldIdentifier { get; set; }
    
    public bool? IsValid
    {
        get
        {
            if (_fieldIdentifier is not { }){ return null; }
            return !EditContext.GetValidationMessages(_fieldIdentifier.Value).Any();
        }
    }
    
    protected override async Task OnInitializedAsync()
    {
        if (EditContext != null && ValueExpression != null)
        { _fieldIdentifier = FieldIdentifier.Create(ValueExpression); }
    }
    
    public void OnValueChanged(T value)
    {
        ValueChanged.InvokeAsync(value);
        if(_fieldIdentifier != null)
        { EditContext?.NotifyFieldChanged(_fieldIdentifier.Value); }
    }
}