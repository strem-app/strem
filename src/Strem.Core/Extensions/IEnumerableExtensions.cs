namespace Strem.Core.Extensions;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> elements, Action<T> action)
    {
        foreach (var element in elements)
        { action(element); }
    }
    
    public static bool HasDuplicate<T>(this IEnumerable<T> source, out T? firstDuplicate)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var checkBuffer = new HashSet<T>();
        foreach (var t in source)
        {
            if (checkBuffer.Add(t))
            {
                continue;
            }

            firstDuplicate = t;
            return true;
        }

        firstDuplicate = default;
        return false;
    }
    
    public static bool AreDistinct<T>(this IEnumerable<T> source)
    {
        return !HasDuplicate(source, out _);
    }
}