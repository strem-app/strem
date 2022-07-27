namespace Strem.Core.State;

public class StateEntry
{
    public const string DefaultContext = "global";
    
    public string Key { get; set; }
    public string Context { get; set; } = DefaultContext;
}