using Microsoft.Extensions.DependencyInjection;

namespace Strem.Core.DI;

public interface IDependencyModule
{
    void Setup(IServiceCollection services);
}