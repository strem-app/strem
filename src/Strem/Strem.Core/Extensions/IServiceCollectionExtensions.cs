using Microsoft.Extensions.DependencyInjection;
using Strem.Core.DI;

namespace Strem.Core.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddModule(this IServiceCollection services, IDependencyModule dependencyModule)
    {
        dependencyModule.Setup(services);
        return services;
    }
        
    public static IServiceCollection AddModule<T>(this IServiceCollection services) where T : IDependencyModule, new()
    { return services.AddModule(new T()); }
}