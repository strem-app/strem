using System.Reflection;

namespace Strem.Core.Extensions;

public static class AppDomainExtensions
{
    public static Assembly[] GetAllAssembliesThatContain<T>(this AppDomain appDomain)
    {
        return appDomain.GetAssemblies()
            .Where(x => x.GetTypes().WhereClassesImplement(typeof(T)).Any())
            .ToArray();
    }
    
    public static Type[] GetAllTypesImplementing<T>(this AppDomain appDomain)
    {
        return appDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes().WhereClassesImplement(typeof(T)))
            .ToArray();
    }
}