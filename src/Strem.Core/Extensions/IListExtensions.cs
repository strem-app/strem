namespace Strem.Core.Extensions;

public static class IListExtensions
{
    public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        return list;
    }
    
    public static IList<T> Swap<T>(this IList<T> list, T elementA, T elementB)
    {
        var indexA = list.IndexOf(elementA);
        var indexB = list.IndexOf(elementB);
        return Swap(list, indexA, indexB);
    }
}