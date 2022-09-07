using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Strem.Core.Extensions;

namespace Strem.Core.Components;

public class ValidatedComponent<T> : ComponentBase, IDisposable
    where T : class
{
    [Parameter]
    public T Data { get; set; }
    
    [Parameter]
    public EventCallback DataChanged { get; set; }
    
    [Parameter]
    public bool ValidateOnStart { get; set; } = true;
    
    [Parameter]
    public EventCallback<bool> IsValidChanged { get; set; }
    
    public EditContext EditContext { get; private set; }

    protected bool _isValid = true;
    public bool IsValid
    {
        get => _isValid;
        private set
        {
            if (_isValid == value) return;
            _isValid = value;
            IsValidChanged.InvokeAsync(value);
        }
    }
    
    protected CompositeDisposable _subs = new();
    
    protected override async Task OnInitializedAsync()
    {
        EditContext = new EditContext(Data);
        EditContext.EnableDataAnnotationsValidation().AddTo(_subs);
        
        Observable.FromEventPattern<ValidationStateChangedEventArgs>(
                x => EditContext.OnValidationStateChanged += x,
                x => EditContext.OnValidationStateChanged -= x)
            .Subscribe(x => OnValidationStateChanged())
            .AddTo(_subs);

        Observable.FromEventPattern<FieldChangedEventArgs>(
                x => EditContext.OnFieldChanged += x,
                x => EditContext.OnFieldChanged -= x)
            .Subscribe(x => OnFieldChanged())
            .AddTo(_subs);
        
        if(ValidateOnStart)
        { IsValid = EditContext.Validate(); }
    }
    
    private void OnFieldChanged()
    { DataChanged.InvokeAsync(); }

    private void OnValidationStateChanged()
    {
        IsValid = !EditContext.GetValidationMessages().Any();
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    { _subs.Dispose(); }
}