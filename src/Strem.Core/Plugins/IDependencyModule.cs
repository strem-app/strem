namespace Strem.Core.Plugins;

public interface IDependencyModule
{
    void Setup(IServiceCollection services);
}