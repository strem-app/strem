namespace Strem.Core.Extensions;

public static class UnmatchedAttributeExtensions
{
    public static IReadOnlyDictionary<string, object> Only(
        this IReadOnlyDictionary<string, object>? availableAttributes, params string[] attributesToUse)
    {
        if (availableAttributes == null)
        { return new Dictionary<string, object>(); }
        
        return availableAttributes
            .Where(x => attributesToUse.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value);
    }
    
    public static IReadOnlyDictionary<string, object> AllBut(
        this IReadOnlyDictionary<string, object>? availableAttributes, params string[] omittedAttributes)
    {
        if (availableAttributes == null)
        { return new Dictionary<string, object>(); }
        
        return availableAttributes
            .Where(x => !omittedAttributes.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public static object ValueFor(this IReadOnlyDictionary<string, object>? availableAttributes, string attribute)
    {
        if (availableAttributes == null || availableAttributes.Count == 0)
        { return string.Empty; }
        
        var actualAttribute = availableAttributes
            .SingleOrDefault(x => x.Key.Equals(attribute, StringComparison.OrdinalIgnoreCase));

        return actualAttribute.Value ?? string.Empty;
    }
}