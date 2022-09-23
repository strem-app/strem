using Strem.Core.Extensions;
using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Variables;
using Strem.OBS.Components.Integrations;
using Strem.OBS.Variables;

namespace Strem.OBS.Plugin;

public class OBSIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "OBS Websocket Integration";
    public string Code => "obs-integration";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        OBSVars.CurrentScene.ToDescriptor(), OBSVars.SourceItems.ToDescriptor()
    };

    public Type ComponentType { get; } = typeof(OBSIntegration);
}