namespace Strem.Core.Services.UI.Modal;

public interface IModalInteractionService
{
    public IObservable<string> OnModalClosed { get; }
    
    public void ShowModal(ModalReference reference);
    public void CloseModal();
}