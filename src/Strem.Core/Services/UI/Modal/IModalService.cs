using System.Reactive;

namespace Strem.Core.Services.UI.Modal;

public interface IModalService : IModalInteractionService, IDisposable
{
    public IObservable<ModalReference> OnModalOpenRequest { get; }
    public IObservable<Unit> OnModalCloseRequest { get; }

    public void ClosingModal(string id);
}