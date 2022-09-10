namespace Strem.Core.Variables;

public record VariableEntry
{
    public const string DefaultContext = "";

    public string Name { get; }
    public string Context { get; }

    public VariableEntry(string name, string? context = DefaultContext)
    {
        Name = name;
        Context = context ?? DefaultContext;
    }
    
    public virtual bool Equals(VariableEntry? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Context == other.Context;
    }

    public override int GetHashCode()
    { return HashCode.Combine(Name, Context); }
}