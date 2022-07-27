using System.Reflection;
using Strem.Core.DI;

namespace Strem.Infrastructure.Services.Api;

public class ApiHostConfiguration
{
    public IEnumerable<Assembly>? ControllerAssemblies { get; set; }
    public IEnumerable<IDependencyModule> Modules { get; set; }

    public ApiHostConfiguration(IEnumerable<Assembly>? controllerAssemblies, IEnumerable<IDependencyModule> modules)
    {
        ControllerAssemblies = controllerAssemblies;
        Modules = modules;
    }
}