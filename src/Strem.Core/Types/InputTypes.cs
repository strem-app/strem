using SharpHook.Native;

namespace Strem.Core.Types;

public class InputTypes
{
    public static readonly ModifierMask[] Modifiers = new[]
    {
        ModifierMask.LeftShift,
        ModifierMask.RightShift,
        ModifierMask.LeftAlt,
        ModifierMask.RightAlt,
        ModifierMask.LeftCtrl,
        ModifierMask.RightCtrl,
        ModifierMask.LeftMeta,
        ModifierMask.RightMeta
    };

    public static KeyCode[] IgnoredKeys = new[]
    {
        KeyCode.VcLeftShift,
        KeyCode.VcRightShift,
        KeyCode.VcLeftAlt,
        KeyCode.VcRightAlt,
        KeyCode.VcLeftMeta,
        KeyCode.VcRightMeta,
        KeyCode.VcLeftControl,
        KeyCode.VcRightControl
    };

    public static readonly KeyCode[] KeyCodes = Enum.GetValues<KeyCode>()
        .Where(x => !IgnoredKeys.Contains(x))
        .ToArray();
}