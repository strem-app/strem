using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Registries.Integrations;
using Strem.OBS.Components.Integrations;
using Strem.OBS.Variables;

namespace Strem.OBS.Plugin;

public class OBSIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "OBS Integration";
    public string Code => "obs-integration";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        OBSVars.CurrentScene.ToDescriptor(), OBSVars.SourceItems.ToDescriptor()
    };

    public Type ComponentType { get; } = typeof(OBSIntegration);
}