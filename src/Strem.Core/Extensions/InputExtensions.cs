using System.Reactive;
using System.Reactive.Linq;
using SharpHook;
using SharpHook.Native;
using SharpHook.Reactive;

namespace Strem.Core.Extensions;

public static class InputExtensions
{
    public static ModifierMask[] AllMasks = Enum.GetValues<ModifierMask>()
        .Where(x => x != ModifierMask.None)
        .ToArray();
    
    public static string MakeKeyCodeNice(this KeyCode keycode)
    { return keycode.ToString().Replace("Vc", ""); }
    
    public static ModifierMask[] GetActiveMasks(this ModifierMask mask)
    {
        return AllMasks
            .Where(x => mask.HasFlag(x))
            .ToArray();
    }
    
    public static bool HasAnyMasks(this ModifierMask mask, IReadOnlyCollection<ModifierMask> maskKeys)
    { return maskKeys.All(x => mask.HasFlag(x)); }

    public static bool HasOnlyMasks(this ModifierMask mask, IReadOnlyCollection<ModifierMask> maskKeys)
    {
        var activeMasks = GetActiveMasks(mask);
        return activeMasks.Length == maskKeys.Count && activeMasks.All(maskKeys.Contains);
    }
    
    public static bool Matches(this IReadOnlyCollection<KeyCode> activeKeys, IReadOnlyCollection<KeyCode> expected)
    {
        return activeKeys.Count == expected.Count &&
               activeKeys.All(expected.Contains);
    }
    
    public static IObservable<Unit> ListenFor(this IReactiveGlobalHook hook, IReadOnlyCollection<ModifierMask> maskKeys,  IReadOnlyCollection<KeyCode> keys)
    {
        return hook.KeyPressed
            .Where(x => x.RawEvent.Mask.HasOnlyMasks(maskKeys))
            .GroupByUntil(_ => true, _ => Observable.Timer(TimeSpan.FromMilliseconds(150)))
            .SelectMany(x => x.Select(y => y.Data.KeyCode).ToArray())
            .Where(x => x.Matches(keys))
            .Select(x => Unit.Default);
    }

    public static void SimulateKeyPresses(this IEventSimulator eventSimulator, IReadOnlyCollection<KeyCode> keys)
    {
        for (var i = 0; i < keys.Count; i++)
        { eventSimulator.SimulateKeyPress(keys.ElementAt(i)); }
        
        for (var i = keys.Count - 1; i >= 0; i--)
        { eventSimulator.SimulateKeyRelease(keys.ElementAt(i)); }
    }

    public static KeyCode ToKeyCode(this ModifierMask modifier)
    {
        if(modifier == ModifierMask.LeftShift) { return KeyCode.VcLeftShift; }
        if(modifier == ModifierMask.RightShift) { return KeyCode.VcRightShift; }
        if(modifier == ModifierMask.LeftAlt) { return KeyCode.VcLeftAlt; }
        if(modifier == ModifierMask.RightAlt) { return KeyCode.VcRightAlt; }
        if(modifier == ModifierMask.LeftCtrl) { return KeyCode.VcLeftControl; }
        if(modifier == ModifierMask.RightCtrl) { return KeyCode.VcRightControl; }
        if(modifier == ModifierMask.LeftMeta) { return KeyCode.VcLeftMeta; }
        if(modifier == ModifierMask.RightMeta) { return KeyCode.VcRightMeta; }

        return KeyCode.CharUndefined;
    }
    
    public static ModifierMask ToModifier(this KeyCode key)
    {
        if(key == KeyCode.VcLeftShift) { return ModifierMask.LeftShift; }
        if(key == KeyCode.VcRightShift) { return ModifierMask.RightShift; }
        if(key == KeyCode.VcLeftAlt) { return ModifierMask.LeftAlt; }
        if(key == KeyCode.VcRightAlt) { return ModifierMask.RightAlt; }
        if(key == KeyCode.VcLeftControl) { return ModifierMask.LeftCtrl; }
        if(key == KeyCode.VcRightControl) { return ModifierMask.RightCtrl; }
        if(key == KeyCode.VcLeftMeta) { return ModifierMask.LeftMeta; }
        if(key == KeyCode.VcRightMeta) { return ModifierMask.RightMeta; }

        return ModifierMask.None;
    }
    
    public static void SimulateKeyPresses(this IEventSimulator eventSimulator, IReadOnlyCollection<ModifierMask> modifiers, IReadOnlyCollection<KeyCode> keys)
    {
        for (var i = 0; i < modifiers.Count; i++)
        { eventSimulator.SimulateKeyPress(modifiers.ElementAt(i).ToKeyCode()); }
        
        for (var i = 0; i < keys.Count; i++)
        { eventSimulator.SimulateKeyPress(keys.ElementAt(i)); }
        
        for (var i = keys.Count - 1; i >= 0; i--)
        { eventSimulator.SimulateKeyRelease(keys.ElementAt(i)); }
        
        for (var i = modifiers.Count - 1; i >= 0; i--)
        { eventSimulator.SimulateKeyRelease(modifiers.ElementAt(i).ToKeyCode()); }
    }
}