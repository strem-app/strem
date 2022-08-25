namespace Strem.Core.DI;

public interface IDependencyModule
{
    void Setup(IServiceCollection services);
}