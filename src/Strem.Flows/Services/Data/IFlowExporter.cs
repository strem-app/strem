namespace Strem.Flows.Services.Data;

public interface IFlowExporter
{
    string Export(IEnumerable<Guid> ids);
}