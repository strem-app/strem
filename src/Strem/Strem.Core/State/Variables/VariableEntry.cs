namespace Strem.Core.State;


public struct VariableEntry
{
    public const string DefaultContext = "global";

    public readonly string Name;
    public readonly string Context;

    public VariableEntry(string name, string context = DefaultContext)
    {
        Name = name;
        Context = context;
    }
}