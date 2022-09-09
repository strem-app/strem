namespace Strem.Core.Extensions;

public static class TypeExtensions
{
    public static Type[] WhereClassesImplement(this IEnumerable<Type> types, Type interfaceType)
    {
        return types
            .Where(p => p.IsClass && !p.IsAbstract && p.GetInterfaces().Any(i => i == interfaceType))
            .ToArray();
    }
    
    public static Type[] WhereClassesInheritFrom(this IEnumerable<Type> types, Type baseClassType)
    {
        return types
            .Where(p => !p.IsInterface && !p.IsAbstract && baseClassType.IsAssignableFrom(p))
            .ToArray();
    }
}