using Strem.Core.Extensions;
using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Variables;
using Strem.OBS.v4.Variables;
using Strem.OBS.v4.Components.Integrations;

namespace Strem.OBS.v4.Plugin;

public class OBSIntegrationDescriptor : IIntegrationDescriptor
{
    public string Title => "OBS Websocket (v4) Integration - (Requires Custom Obs-Websocket v4 Plugin For OBS)";
    public string Code => "obs-integration-v4";

    public VariableDescriptor[] VariableOutputs { get; } = new[]
    {
        OBSVars.CurrentScene.ToDescriptor(), OBSVars.SourceItems.ToDescriptor()
    };

    public Type ComponentType { get; } = typeof(OBSIntegration);
}