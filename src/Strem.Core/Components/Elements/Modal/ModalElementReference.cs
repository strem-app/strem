using Microsoft.AspNetCore.Components;
using Strem.Core.Services.UI.Modal;

namespace Strem.Core.Components.Elements.Modal;

public abstract class ModalElementReference : ComponentBase
{
    public abstract ModalReference Reference { get; protected set; }
}