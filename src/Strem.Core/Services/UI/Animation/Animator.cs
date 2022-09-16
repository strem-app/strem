using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Strem.Core.Services.UI.Animation;

public class Animator : IAnimator
{
    public IJSRuntime JsRuntime { get; }

    public Animator(IJSRuntime jsRuntime)
    { JsRuntime = jsRuntime; }

    public async Task Animate(ElementReference element, string animationType)
    { await JsRuntime.InvokeVoidAsync("animateElement", element, animationType); }
}