namespace Strem.Core.Components.Elements.Inputs;

public class ValueWithPrevious<T>
{
    public T Previous { get; set; }
    public T New { get; set; }

    public ValueWithPrevious(T previous, T @new)
    {
        Previous = previous;
        New = @new;
    }
}