using Microsoft.AspNetCore.Components;

namespace Strem.Core.Services.UI.Modal;

public class ModalReference
{
    public string Id { get; set; }
    public RenderFragment ModalContent { get; set; }

    public ModalReference(string id, RenderFragment modalContent)
    {
        Id = id;
        ModalContent = modalContent;
    }
}