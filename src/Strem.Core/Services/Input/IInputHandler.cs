using System.Reactive;
using SharpHook.Native;

namespace Strem.Core.Services.Input;

public interface IInputHandler : IDisposable
{
    void SimulateKeyPress(params KeyCode[] keyCodes);
    void SimulateModifiedKeyPress(IReadOnlyCollection<ModifierMask> modifiers, IReadOnlyCollection<KeyCode> keyCodes);
    IObservable<Unit> ListenForKeyPresses(IReadOnlyCollection<ModifierMask> modifiers, IReadOnlyCollection<KeyCode> keyCodes);
    void StartInputHook();
}