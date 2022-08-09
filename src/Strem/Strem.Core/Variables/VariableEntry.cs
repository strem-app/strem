namespace Strem.Core.Variables;

public struct VariableEntry : IEquatable<VariableEntry>
{
    public const string DefaultContext = "";

    public readonly string Name;
    public readonly string Context;

    public VariableEntry(string name, string context = DefaultContext)
    {
        Name = name;
        Context = context;
    }

    public bool Equals(VariableEntry other)
    {
        return Name == other.Name && Context == other.Context;
    }

    public override bool Equals(object? obj)
    {
        return obj is VariableEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Context);
    }

    public static bool operator ==(VariableEntry left, VariableEntry right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableEntry left, VariableEntry right)
    {
        return !left.Equals(right);
    }
}