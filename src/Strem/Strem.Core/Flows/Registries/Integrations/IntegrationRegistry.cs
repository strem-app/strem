namespace Strem.Core.Flows.Registries.Integrations;

public class IntegrationRegistry : IIntegrationRegistry
{
    public Dictionary<string, IIntegrationDescriptor> Integrations { get; } = new();
    public void Add(IIntegrationDescriptor integration) => Integrations.Add(integration.Code, integration);
    public void Remove(IIntegrationDescriptor integration) => Integrations.Remove(integration.Code);
    public IIntegrationDescriptor Get(string integrationCode) => Integrations.ContainsKey(integrationCode) ? Integrations[integrationCode] : null;
    public IEnumerable<IIntegrationDescriptor> GetAll() => Integrations.Values;
}