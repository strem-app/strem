using System.Reactive;
using SharpHook;
using SharpHook.Native;
using SharpHook.Reactive;
using Strem.Core.Extensions;

namespace Strem.Core.Services.Input;

public class DefaultInputHandler : IInputHandler
{
    public IReactiveGlobalHook InputHook { get; }
    public IEventSimulator InputSimulator { get; }

    public DefaultInputHandler(IReactiveGlobalHook inputHook, IEventSimulator inputSimulator)
    {
        InputHook = inputHook;
        InputSimulator = inputSimulator;
    }

    public void StartInputHook()
    { InputHook.RunAsync(); }

    public void Dispose()
    { InputHook.Dispose(); }

    public void SimulateKeyPress(params KeyCode[] keyCodes)
    { InputSimulator.SimulateKeyPresses(keyCodes); }

    public void SimulateModifiedKeyPress(IReadOnlyCollection<ModifierMask> modifiers, IReadOnlyCollection<KeyCode> keyCodes)
    { InputSimulator.SimulateKeyPresses(modifiers, keyCodes); }

    public IObservable<Unit> ListenForKeyPresses(IReadOnlyCollection<ModifierMask> modifiers, IReadOnlyCollection<KeyCode> keyCodes)
    { return InputHook.ListenFor(modifiers, keyCodes); }
}