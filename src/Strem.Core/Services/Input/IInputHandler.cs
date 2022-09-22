using Strem.Core.Types;
using Strem.Core.Types.Input;

namespace Strem.Core.Services.Input;

public interface IInputHandler
{
    void KeyPress(params InputKeyCodes[] keyCodes);
    void ModifiedKeyPress(IEnumerable<InputKeyCodes> modifierKeyCodes, IEnumerable<InputKeyCodes> keyCodes);
}