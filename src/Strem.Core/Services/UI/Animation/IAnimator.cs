using Microsoft.AspNetCore.Components;

namespace Strem.Core.Services.UI.Animation;

public interface IAnimator
{
    Task Animate(ElementReference element, string animationType);
}