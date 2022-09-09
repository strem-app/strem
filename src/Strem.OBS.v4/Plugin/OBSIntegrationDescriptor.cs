using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Registries.Integrations;
using Strem.OBS.v4.Variables;
using Strem.OBS.v4.Components.Integrations;

namespace Strem.OBS.v4.Plugin;

public class OBSIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "OBS Integration (v4)";
    public string Code => "obs-integration-v4";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        OBSVars.CurrentScene.ToDescriptor(), OBSVars.SourceItems.ToDescriptor()
    };

    public Type ComponentType { get; } = typeof(OBSIntegration);
}