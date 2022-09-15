namespace Strem.Core.Services.Registries.Integrations;

public class IntegrationRegistry : Registry<IIntegrationDescriptor>, IIntegrationRegistry
{
    public override string GetId(IIntegrationDescriptor data) => data.Code;
}