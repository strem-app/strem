using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using Strem.Core.Services.Input;
using Strem.Core.Types;
using Strem.Core.Types.Input;

namespace Strem.Platforms.Windows.Services.Input;

public class WindowsInputHandler : IInputHandler
{
    public IInputSimulator InputSimulator { get; }

    public WindowsInputHandler(IInputSimulator inputSimulator)
    {
        InputSimulator = inputSimulator;
    }

    public void KeyPress(params InputKeyCodes[] keyCodes)
    {
        var nativeKeyCodes = keyCodes.Cast<VirtualKeyCode>().ToArray();
        InputSimulator.Keyboard.KeyPress(nativeKeyCodes);
    }

    public void ModifiedKeyPress(IEnumerable<InputKeyCodes> modifierKeyCodes, IEnumerable<InputKeyCodes> keyCodes)
    {
        var nativeModifierCodes = modifierKeyCodes.Cast<VirtualKeyCode>();
        var nativeKeyCodes = keyCodes.Cast<VirtualKeyCode>().ToArray();
        InputSimulator.Keyboard.ModifiedKeyStroke(nativeModifierCodes, nativeKeyCodes);
    }
}